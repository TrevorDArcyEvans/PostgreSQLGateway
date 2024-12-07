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

              // back end key
              var keyData = new BackendKeyDataMessage
              {
                ProcessId = Environment.ProcessId,
                SecretKey = Random.Shared.Next()
              };
              stream.Write(Serializer.Serialize(keyData));

              // ready for query
              var ready = new ReadyForQueryMessage
              {
                TransactionStatus = (byte)'I' // transaction idle response
              };
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
        }
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

  private void Process(NetworkStream stream, QueryMessage query)
  {
    var rowDescr = new RowDescriptionMessage();
    rowDescr.RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = "Index",
        TableOid = 1,
        RowAttributeId = 0,
        FieldTypeOid = 0,
        DataTypeSize = 4,
        TypeModifier = 0,
        FormatCode = 1
      });
    rowDescr.RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = "CustomerName",
        TableOid = 1,
        RowAttributeId = 0,
        FieldTypeOid = 0,
        DataTypeSize = -1,
        TypeModifier = 0,
        FormatCode = 1
      });
    stream.Write(Serializer.Serialize(rowDescr));


    var dataRow = new DataRowMessage();

    // index
    var idxData = BitConverter.GetBytes(21).Reverse().ToArray();
    var idx = new RowField
    {
      Length = idxData.Length,
      Value = idxData
    };
    dataRow.Rows.Add(idx);

    // customer name
    var bytes = new List<byte>();

    bytes.AddRange("Mr Jacob Rees-Mogg Esq"u8.ToArray());
    bytes.Add(0);

    var nameData = bytes.ToArray();
    var name = new RowField
    {
      Length = nameData.Length,
      Value = nameData
    };
    dataRow.Rows.Add(name);

    stream.Write(Serializer.Serialize(dataRow));


    var complete = new CommandCompleteMessage
    {
      CommandTag = 1.ToString()
    };
    stream.Write(Serializer.Serialize(complete));


    var ready = new ReadyForQueryMessage
    {
      TransactionStatus = (byte)'I' // transaction idle response
    };
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
      ;
    }

    _logger.LogInformation("Parser Fail");

    return Task.CompletedTask;
  }
}
