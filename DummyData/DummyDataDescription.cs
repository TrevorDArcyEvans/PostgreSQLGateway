namespace DummyData;

using PostgresMessageSerializer;

public class DummyDataDescription : RowDescriptionMessage<DummyDataMessage>
{
  public DummyDataDescription()
  {
    RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = nameof(DummyDataMessage.Id),
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = (int)ColumnTypeExtensions.TypeToColumnTypeMap[typeof(DummyDataMessage).GetProperty(nameof(DummyDataMessage.Id)).PropertyType],
        DataTypeSize = sizeof(int),
        TypeModifier = -1,
        FormatCode = 0
      });
    RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = nameof(DummyDataMessage.Name),
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = (int)ColumnTypeExtensions.TypeToColumnTypeMap[typeof(DummyDataMessage).GetProperty(nameof(DummyDataMessage.Name)).PropertyType],
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
  }
}
