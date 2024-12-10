namespace PostgreSQLGateway.JDBCMetadata;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class ParseMessageHandler: IMessageHandler<ParseMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, ParseMessage message)
  {
    return false;
  }
}
