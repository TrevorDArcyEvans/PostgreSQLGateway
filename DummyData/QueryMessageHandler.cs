namespace DummyData;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class QueryMessageHandler : IMessageHandler<QueryMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage message)
  {
    var stopProcessing = true;

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

      // version
      var version = new RowField
      {
        // WTF?  Have to serialise int32 as string
        Value = SerializerCore.Serialize(21.ToString())
      };
      dataRow.Rows.Add(version);

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

    return stopProcessing;
  }
}
