namespace PostgreSQLGateway.Interfaces;

using System.Net.Sockets;
using PostgresMessageSerializer;

public interface IMessageHandler<in T> where T : Message
{
  /// <summary>
  /// Interface to handle a front end message and send results or
  /// other info back to front end client
  /// </summary>
  /// <param name="stream">network connection to front end client</param>
  /// <param name="startupMsg">initial Startup message from front end</param>
  /// <param name="message">message from front end</param>
  /// <returns>True if further processing of this message should stop</returns>
  bool Process(NetworkStream stream, StartupMessage startupMsg, T message);
}
