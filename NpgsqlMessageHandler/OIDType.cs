namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _SELECT_ns_nspname_t_oid_t_typname_t_typtype_t_typnotnull_t_elem_202412151616.csv
/// </summary>
public class OIDType : DataRowMessage
{
  public string nspname { get; set; }
  public int oid { get; set; }
  public string typname { get; set; }
  public string typtype { get; set; }
  public bool typnotnull { get; set; }
  public int elemtypoid { get; set; }
}
