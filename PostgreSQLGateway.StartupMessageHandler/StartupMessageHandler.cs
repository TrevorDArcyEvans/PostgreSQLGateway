namespace PostgreSQLGateway.StartupMessageHandler;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class StartupMessageHandler : IMessageHandler<StartupMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, StartupMessage message)
  {
    // authentication OK
    var authOk = new AuthenticationMessage();
    stream.Write(Serializer.Serialize(authOk));

    // parameter statuses
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("application_name", "PostgreSQLGateway")));
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("client_encoding", "UTF8")));
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("server_version", "17.2")));
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("server_encoding", "UTF8")));
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("DateStyle", "ISO")));
    stream.Write(Serializer.Serialize(new ParameterStatusMessage("PreferQueryMode", "simple")));

    // back end key
    var keyData = new BackendKeyDataMessage
    {
      ProcessId = Environment.ProcessId,
      SecretKey = stream.GetHashCode()
    };
    stream.Write(Serializer.Serialize(keyData));

    // ready for query
    var ready = new ReadyForQueryMessage();
    stream.Write(Serializer.Serialize(ready));

    return true;
  }
}
