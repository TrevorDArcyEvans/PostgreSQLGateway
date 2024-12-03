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
      // Start listening for incoming connections
      listener.Start();
      _logger.LogInformation("Listening for connections on port {0}...", port);

      // Accept incoming connections
      using var client = listener.AcceptTcpClient();
      _logger.LogInformation("Connection accepted from {0}.", client.Client.RemoteEndPoint);
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
        _logger.LogInformation("Received data from client: {0}", receivedData);

        // grand central dispatch from here
      }
    }
    catch (Exception ex)
    {
      _logger.LogInformation("An error occurred: {0}", ex.Message);
    }
    finally
    {
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
