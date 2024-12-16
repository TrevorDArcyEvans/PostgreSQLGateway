namespace NpgsqlMessageHandler;

/// <summary>
/// Data from:
///     _Load_enum_fields_SELECT_pg_type_oid_enumlabel_FROM_pg_enum_JOIN_202412151617.csv
/// </summary>
public class EnumField
{
  public int oid { get; set; } // 26 = OID
  public string enumlabel { get; set; } // 19 = NAME
}