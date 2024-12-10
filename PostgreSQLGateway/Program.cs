namespace PostgreSQLGateway;

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

internal class Program
{
  private readonly IEnumerable<IMessageHandler<QueryMessage>?> _queryMessageHandlers;
  private readonly IEnumerable<IMessageHandler<ParseMessage>?> _parseMessageHandlers;

  private readonly ILogger<Program> _logger;

  public static async Task Main(string[] args)
  {
    var prog = new Program(args);
    await prog.Run(args);
  }

  private Program(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();
    var appConfig = app.Configuration;
    var logLevelStr = appConfig["Logging:LogLevel:Default"];
    var logLevel = Enum.Parse<LogLevel>(logLevelStr);
    var services = new ServiceCollection();

    services.AddLogging(build =>
    {
      build.SetMinimumLevel(logLevel);
      build.AddConsole();
    });

    var serviceProvider = services.BuildServiceProvider();
    _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();


    var messageHandlers = appConfig.GetSection("MessageHandlers");

    var queryMH = messageHandlers.GetSection("QueryMessage");
    var queryMHlist = new List<MessageHandlerConfig>();
    queryMH.Bind(queryMHlist);
    _queryMessageHandlers = queryMHlist
      .OrderBy(x => x.Order)
      .Select(x =>
      {
        var type = Assembly
          .LoadFile(Path.Combine(Environment.CurrentDirectory, x.Assembly))
          .GetTypes()
          .Single(t => t.FullName == x.Type);
        return (IMessageHandler<QueryMessage>)Activator.CreateInstance(type);
      });


    var parseMH = messageHandlers.GetSection("ParseMessage");
    var parseMHlist = new List<MessageHandlerConfig>();
    parseMH.Bind(parseMHlist);
    _parseMessageHandlers = parseMHlist
      .OrderBy(x => x.Order)
      .Select(x => 
      {
        var type = Assembly
          .LoadFile(Path.Combine(Environment.CurrentDirectory, x.Assembly))
          .GetTypes()
          .Single(t => t.FullName == x.Type);
        return (IMessageHandler<ParseMessage>)Activator.CreateInstance(type);
      });
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
    using var listener = new TcpListener(ipAddress, port);

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

      // accept incoming connections
      while (isRunning)
      {
        while (!listener.Pending())
        {
          Thread.Sleep(10);

          if (!isRunning)
          {
            return;
          }
        }

        var client = await listener.AcceptTcpClientAsync();
        var t = new Thread(HandleClient);
        t.Start(client);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred: {ex.Message}");
    }
    finally
    {
      _logger.LogInformation("Shutting down...");

      // Stop listening for connections
      listener.Stop();
    }
  }

  private async void HandleClient(object? obj)
  {
    try
    {
      using var client = (TcpClient)obj!;

      _logger.LogInformation("Connection accepted from {0}.", client.Client.RemoteEndPoint);
      await using var stream = client.GetStream();

      var startupMsg = new StartupMessage();

      var isRunning = true;
      while (isRunning)
      {
        // receive data from the client
        var buffer = new byte[1024 * 1024];
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
            if (value == SSLRequestMessage.SSLRequestMessageId)
            {
              // SSL declined
              stream.Write([(byte)'N']);
              continue;
            }

            // startup request
            if (value == StartupMessage.ProtocolVersion)
            {
              // startup message is length prefixed, so start buffer after size
              startupMsg.Deserialize(buffer[sizeof(int)..bytesRead]);

              var smh = new StartupMessageHandler();
              _ = smh.Process(stream, startupMsg, startupMsg);

              continue;
            }
          }
        }

        // deserialise front end message
        var msg = Serializer.DeserializeFrontEnd(buffer[..bytesRead]);
        switch (msg)
        {
          case QueryMessage query:
            Process(stream, startupMsg, query);
            break;

          case ParseMessage parse:
            Process(stream, startupMsg, parse);
            break;

          case TerminateMessage terminate:
            isRunning = false;
            break;

          default:
            _logger.LogError($"Unknown message: {msg.MessageTypeId} / {msg.GetType().Name}");
            break;
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred: {ex.Message}");
    }
  }

  private void Process(NetworkStream stream, StartupMessage startupMsg, ParseMessage parse)
  {
    _logger.LogInformation(parse.Query);

    foreach (var pmh in _parseMessageHandlers)
    {
      if (pmh.Process(stream, startupMsg, parse))
      {
        return;
      }
    }
  }

  private void Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage query)
  {
    foreach (var qmh in _queryMessageHandlers)
    {
      if (qmh.Process(stream, startupMsg, query))
      {
        return;
      }
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
    }

    _logger.LogInformation("Parser Fail");

    return Task.CompletedTask;
  }
}
