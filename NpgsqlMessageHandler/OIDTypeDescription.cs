namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

public class OIDTypeDescription : RowDescriptionMessage<OIDType>
{
  public OIDTypeDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.NAME, nameof(OIDType.nspname)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.OID, nameof(OIDType.oid)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.NAME, nameof(OIDType.typname)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.CHAR, nameof(OIDType.typtype)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.BOOL, nameof(OIDType.typnotnull)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.OID, nameof(OIDType.elemtypoid)));
  }
}
