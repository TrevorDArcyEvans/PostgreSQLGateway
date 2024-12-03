using System.Buffers;
using PostgresMessageSerializer;

namespace PostgreSQLGateway;

using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommandLine;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    // Specify the IP address and port to listen on
    var ipAddress = IPAddress.Parse("0.0.0.0");
    var port = 5432;

    // Create a TCP listener
    var listener = new TcpListener(ipAddress, port);

    var serialiser = new Serializer();

    try
    {
      // Start listening for incoming connections
      listener.Start();
      Console.WriteLine("Listening for connections on port {0}...", port);

      // Accept incoming connections
      using var client = listener.AcceptTcpClient();
      Console.WriteLine("Connection accepted from {0}.", client.Client.RemoteEndPoint);
      var stream = client.GetStream();
      var isRunning = true;
      while (isRunning)
      {
        // receive data from the client
        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead <= 0)
        {
          continue;
        }
        
        
        // TODO   check for SSL request et al
        var seq = new ReadOnlySequence<byte>(buffer[..bytesRead]);
        var reader = new SequenceReader<byte>(seq);
        var ok1 = reader.TryReadBigEndian(out int length);
        var ok2 = reader.TryReadBigEndian(out int value);

        
        var msg = serialiser.Deserialize(buffer[0..bytesRead]);

        var receivedData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received data from client: {0}", receivedData);

        // grand central dispatch from here
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("An error occurred: {0}", ex.Message);
    }
    finally
    {
      // Stop listening for connections
      listener.Stop();
    }
  }

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
      ;
    }

    Console.WriteLine("Parser Fail");
    return Task.CompletedTask;
  }
}
