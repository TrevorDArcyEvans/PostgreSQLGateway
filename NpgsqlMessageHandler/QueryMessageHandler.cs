namespace NpgsqlMessageHandler;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class QueryMessageHandler : IMessageHandler<QueryMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage query)
  {
    if (!query.Query.StartsWith("SELECT version();"))
    {
      return false;
    }

    var stopProcessing = false;

    // SELECT version();
    var rowDescr = new RowDescriptionMessage();
    rowDescr.RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = "version",
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = 1043, // varchar
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
    stream.Write(Serializer.Serialize(rowDescr));

    // version
    var dataRow = new DataRowMessage();
    var version = new RowField
    {
      Value = SerializerCore.Serialize("PostgreSQL 17.2 (Debian 17.2-1.pgdg120+1) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit")
    };
    dataRow.Rows.Add(version);
    stream.Write(Serializer.Serialize(dataRow));


    var complete = new CommandCompleteMessage
    {
      CommandTag = 1.ToString()
    };
    stream.Write(Serializer.Serialize(complete));

    stream.Write(Serializer.Serialize(rowDescr));
    stream.Write(Serializer.Serialize(complete));

    // var ready = new ReadyForQueryMessage();
    // stream.Write(Serializer.Serialize(ready));

    return stopProcessing;
  }
}
