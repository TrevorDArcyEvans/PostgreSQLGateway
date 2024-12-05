namespace PostgresMessageSerializer;

public class RowFieldDescription
{
  public string FieldName { get; set; }
  public int TableOid { get; set; }
  public short RowAttributeId { get; set; }
  public int FieldTypeOid { get; set; }
  public short DataTypeSize { get; set; }
  public int TypeModifier { get; set; }
  public short FormatCode { get; set; }
}
