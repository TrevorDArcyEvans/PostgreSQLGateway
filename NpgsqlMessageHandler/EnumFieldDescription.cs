namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

public class EnumFieldDescription : RowDescriptionMessage<EnumField>
{
  public EnumFieldDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.OID, nameof(EnumField.oid)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.NAME, nameof(EnumField.enumlabel)));
  }
}
