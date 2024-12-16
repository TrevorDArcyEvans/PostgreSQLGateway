namespace DummyData;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class QueryMessageHandler : IMessageHandler<QueryMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage message)
  {
    var stopProcessing = true;

    var rowDescr = new DummyDataDescription();
    stream.Write(Serializer.Serialize(rowDescr));


    var dr1 = new DummyDataMessage(21, "Mr Jacob Rees-Mogg Esq");
    var dr2 = new DummyDataMessage(23, "Rishi Sunak");
    stream.Write(Serializer.Serialize(dr1));
    stream.Write(Serializer.Serialize(dr2));


    var complete = new CommandCompleteMessage
    {
      CommandTag = 1.ToString()
    };
    stream.Write(Serializer.Serialize(complete));


    var ready = new ReadyForQueryMessage();
    stream.Write(Serializer.Serialize(ready));

    return stopProcessing;
  }
}
