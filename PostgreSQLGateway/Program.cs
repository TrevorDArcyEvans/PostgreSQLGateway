namespace PostgreSQLGateway;

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using CommandLine;
using PostgresMessageSerializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
  private readonly ILogger<Program> _logger;

  public static async Task Main(string[] args)
  {
    var prog = new Program(args);
    await prog.Run(args);
  }

  public Program(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();
    var appConfig = app.Configuration;
    var logLevelStr = appConfig["Logging:LogLevel:Default"];
    var logLevel = Enum.Parse<LogLevel>(logLevelStr);
    var services = new ServiceCollection();

    services.AddLogging(builder =>
    {
      builder.SetMinimumLevel(logLevel);
      builder.AddConsole();
    });

    var serviceProvider = services.BuildServiceProvider();
    _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
  }

  private async Task Run(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private async Task Run(Options opt)
  {
    // Specify the IP address and port to listen on
    var ipAddress = IPAddress.Parse("0.0.0.0");
    var port = opt.Port;

    // Create a TCP listener
    var listener = new TcpListener(ipAddress, port);


    var serialiser = new Serializer();


    try
    {
      // ctrl-c in console to exit
      var isRunning = true;
      Console.CancelKeyPress += (sender, e) =>
      {
        isRunning = false;
        e.Cancel = true;
      };
      _logger.LogInformation("Starting server - ctrl-c to stop...");

      // start listening for incoming connections
      listener.Start();
      _logger.LogInformation("Listening for connections on port {0}...", port);

      while (!listener.Pending())
      {
        Thread.Sleep(10);

        if (!isRunning)
        {
          return;
        }
      }

      // accept incoming connections
      using var client = await listener.AcceptTcpClientAsync();
      _logger.LogInformation("Connection accepted from {0}.", client.Client.RemoteEndPoint);
      await using var stream = client.GetStream();

      while (isRunning)
      {
        // receive data from the client
        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead <= 0)
        {
          continue;
        }


        var receivedData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
        _logger.LogInformation($"Received data from client: {receivedData}");

        // grand central dispatch from here

        // TODO   check for SSL request et al
        var seq = new ReadOnlySequence<byte>(buffer[..bytesRead]);
        var reader = new SequenceReader<byte>(seq);
        if (reader.TryReadBigEndian(out int length) && length >= 8)
        {
          if (reader.TryReadBigEndian(out int value))
          {
            // SSL request
            if (value == 80877103)
            {
              // SSL declined
              stream.Write([(byte)'N']);
              continue;
            }

            // startup request
            if (value == 0x30000) // 196608
            {
              // authentication OK
              stream.Write([
                (byte)'R',
                0x00, 0x00, 0x00, 0x08,
                0x00, 0x00, 0x00, 0x00
              ]);

              // back end key
              stream.Write([
                (byte)'K',
                0x00, 0x00, 0x00, 0x0c,
                0x00, 0x00, 0x04, 0xd2,
                0x00, 0x00, 0x16, 0x2e
              ]);

              // ready for query
              stream.Write([
                (byte)'Z',
                0x00, 0x00, 0x00, 0x05,
                (byte)'I' // transaction idle response
              ]);

              continue;
            }
          }
        }

        // TODO   deserialise front end message
        var msg = serialiser.Deserialize(buffer[..bytesRead]);
      }
    }
    catch (Exception ex)
    {
      _logger.LogInformation("An error occurred: {0}", ex.Message);
    }
    finally
    {
      _logger.LogInformation("Shutting down...");

      // Stop listening for connections
      listener.Stop();
    }
  }

  private Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      _logger.LogInformation("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      _logger.LogInformation("Help Request");
      return Task.CompletedTask;
      ;
    }

    _logger.LogInformation("Parser Fail");

    return Task.CompletedTask;
  }
}
