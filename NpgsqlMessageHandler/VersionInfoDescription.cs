namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

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
        FieldTypeOid = (int)ColumnTypeExtensions.TypeToColumnTypeMap[typeof(VersionInfo).GetProperty(nameof(VersionInfo.version)).PropertyType],
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
  }
}
