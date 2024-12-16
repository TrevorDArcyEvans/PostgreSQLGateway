namespace DummyData;

using PostgresMessageSerializer;

public class DummyDataDescription : RowDescriptionMessage
{
  public DummyDataDescription()
  {
    RowFieldDescriptions.Add(
      new RowFieldDescription
      {
        FieldName = nameof(DummyDataMessage.Id),
        TableOid = 0,
        RowAttributeId = 0,
        FieldTypeOid = (int)ColumnType.INT4, // int32
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
        FieldTypeOid = (int)ColumnType.TEXT, // string
        DataTypeSize = -1,
        TypeModifier = -1,
        FormatCode = 0
      });
  }
}
