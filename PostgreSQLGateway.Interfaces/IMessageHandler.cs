namespace PostgreSQLGateway.Interfaces;

using System.Net.Sockets;
using PostgresMessageSerializer;

public interface IMessageHandler<in T> where T : Message
{
  bool Process(NetworkStream stream, StartupMessage startupMsg, T message);
}
