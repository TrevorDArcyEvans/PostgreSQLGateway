namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

public class FieldDefinitionDescription : RowDescriptionMessage<FieldDefinition>
{
  public FieldDefinitionDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.OID, nameof(FieldDefinition.oid)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.NAME, nameof(FieldDefinition.attname)));
    RowFieldDescriptions.Add(new RowFieldDescription(ColumnType.OID, nameof(FieldDefinition.atttypid)));
  }
}
