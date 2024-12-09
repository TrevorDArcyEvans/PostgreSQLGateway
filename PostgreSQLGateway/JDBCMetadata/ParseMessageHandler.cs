namespace PostgreSQLGateway.JDBCMetadata;

using System.Net.Sockets;
using PostgresMessageSerializer;

public class ParseMessageHandler: IMessageHandler<ParseMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, ParseMessage message)
  {
    return false;
  }
}
