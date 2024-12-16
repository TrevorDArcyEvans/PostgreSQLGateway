namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _Load_field_definitions_for_free_standing_composite_types_SELECT_202412151616.csv
/// </summary>
public class FieldDefinition : DataRowMessage
{
  public int oid { get; set; } // 26 = OID
  public string attname { get; set; } // 19 = NAME
  public int atttypid { get; set; } // 26 = OID

  public override void Update()
  {
    Rows.Clear();

    Rows.Add(new RowField {Value = SerializerCore.Serialize(oid)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(attname)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(atttypid)});
  }
}
