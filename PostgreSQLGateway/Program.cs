namespace PostgreSQLGateway;

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using CommandLine;
using PostgresMessageSerializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
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
              // authentication OK
              var authOk = new AuthenticationMessage();
              stream.Write(Serializer.Serialize(authOk));

              // parameter statuses
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("application_name", "PostgreSQLGateway")));
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("client_encoding", "UTF8")));
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("server_version", "14.7")));
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("server_encoding", "UTF8")));
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("DateStyle", "ISO")));
              stream.Write(Serializer.Serialize(new ParameterStatusMessage("PreferQueryMode", "simple")));

              // back end key
              var keyData = new BackendKeyDataMessage
              {
                ProcessId = Environment.ProcessId,
                SecretKey = Random.Shared.Next()
              };
              stream.Write(Serializer.Serialize(keyData));

              // ready for query
              var ready = new ReadyForQueryMessage();
              stream.Write(Serializer.Serialize(ready));

              continue;
            }
          }
        }

        // deserialise front end message
        var msg = Serializer.DeserializeFrontEnd(buffer[..bytesRead]);
        switch (msg)
        {
          case QueryMessage query:
            Process(stream, query);
            break;

          case TerminateMessage terminate:
            isRunning = false;
            break;

          default:
            _logger.LogError($"Unknown message: {msg.MessageTypeId}");
            break;
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred: {ex.Message}");
    }
  }

  private void Process(NetworkStream stream, QueryMessage query)
  {
    var rowDescr = new RowDescriptionMessage();
    rowDescr.RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = "Id",
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = 23, // int32
        DataTypeSize = sizeof(int),
        TypeModifier = -1,
        FormatCode = 0
      });
    rowDescr.RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = "Name",
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = 25, // string
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
    stream.Write(Serializer.Serialize(rowDescr));


    {
      var dataRow = new DataRowMessage();

      // index
      var idx = new RowField
      {
        // WTF?  Have to serialise int32 as string
        Value = SerializerCore.Serialize(21.ToString())
      };
      dataRow.Rows.Add(idx);

      // customer name
      var name = new RowField
      {
        Value = SerializerCore.Serialize("Mr Jacob Rees-Mogg Esq")
      };
      dataRow.Rows.Add(name);

      stream.Write(Serializer.Serialize(dataRow));
    }

    {
      var dataRow = new DataRowMessage();

      // index
      var idx = new RowField
      {
        // WTF?  Have to serialise int32 as string
        Value = SerializerCore.Serialize(23.ToString())
      };
      dataRow.Rows.Add(idx);

      // customer name
      var name = new RowField
      {
        Value = SerializerCore.Serialize("Rishi Sunak")
      };
      dataRow.Rows.Add(name);

      stream.Write(Serializer.Serialize(dataRow));
    }


    var complete = new CommandCompleteMessage
    {
      CommandTag = 1.ToString()
    };
    stream.Write(Serializer.Serialize(complete));


    var ready = new ReadyForQueryMessage();
    stream.Write(Serializer.Serialize(ready));
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
