namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

public class OIDTypeDescription : RowDescriptionMessage<OIDType>
{
  public OIDTypeDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.nspname)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.oid)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.typname)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.typtype)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.typnotnull)));
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(OIDType), nameof(OIDType.elemtypoid)));
  }
}
