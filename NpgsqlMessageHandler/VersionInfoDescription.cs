using PostgresMessageSerializer;

namespace NpgsqlMessageHandler;

public class VersionInfoDescription : RowDescriptionMessage<VersionInfo>
{
  public VersionInfoDescription()
  {
    RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = nameof(VersionInfo.version),
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = (int)ColumnType.TEXT,
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
  }
}
