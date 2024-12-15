namespace NpgsqlMessageHandler;

using System.Net.Sockets;
using PostgresMessageSerializer;
using PostgreSQLGateway.Interfaces;

public class QueryMessageHandler : IMessageHandler<QueryMessage>
{
  public bool Process(NetworkStream stream, StartupMessage startupMsg, QueryMessage query)
  {
    if (!query.Query.StartsWith("SELECT version();") ||
        !query.Query.Contains("SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid") ||
        !query.Query.Contains("-- Arrays have typtype=b - this subquery identifies them by their typreceive and converts their typtype to a") ||
        !query.Query.Contains("-- We first do this for the type (innerest-most subquery), and then for its element type") ||
        !query.Query.Contains("-- This also returns the array element, range subtype and domain base type as elemtypoid") )
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
        FieldTypeOid = (int) ColumnType.TEXT,
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
      CommandTag = "SELECT 1"
    };
    stream.Write(Serializer.Serialize(complete));

    stream.Write(Serializer.Serialize(rowDescr));
    stream.Write(Serializer.Serialize(complete));

    // var ready = new ReadyForQueryMessage();
    // stream.Write(Serializer.Serialize(ready));

    return stopProcessing;
  }
}
