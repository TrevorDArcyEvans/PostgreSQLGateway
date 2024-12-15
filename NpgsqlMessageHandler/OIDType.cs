namespace NpgsqlMessageHandler;

/// <summary>
/// Data from:
///     _SELECT_ns_nspname_t_oid_t_typname_t_typtype_t_typnotnull_t_elem_202412151616.csv
/// </summary>
public class OIDType
{
  public string nspname { get; set; } // 19 = NAME
  public int oid { get; set; } // 26 = OID
  public string typname { get; set; } // 19 = NAME
  public string typtype { get; set; } // 18 = CHAR
  public bool typnotnull { get; set; } // 16 = BOOL
  public int elemtypoid { get; set; } // 26 = OID
}
