namespace NpgsqlMessageHandler;

using System.Globalization;
using System.Reflection;
using CsvHelper;
using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _SELECT_ns_nspname_t_oid_t_typname_t_typtype_t_typnotnull_t_elem_202412151616.csv
/// </summary>
public class OIDType : DataRowMessage
{
  public static readonly Lazy<List<OIDType>> Instance = new(() =>
  {
    const string DataFile = "_SELECT_ns_nspname_t_oid_t_typname_t_typtype_t_typnotnull_t_elem_202412151616.csv";

    using var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DataFile));
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    csv.Context.RegisterClassMap<OIDTypeMap>();

    return csv.GetRecords<OIDType>().ToList();
  });

  public string nspname { get; set; } // 19 = NAME
  public int oid { get; set; } // 26 = OID
  public string typname { get; set; } // 19 = NAME
  public string typtype { get; set; } // 18 = CHAR
  public bool typnotnull { get; set; } // 16 = BOOL
  public int elemtypoid { get; set; } // 26 = OID

  public override void Update()
  {
    Rows.Clear();

    Rows.Add(new RowField {Value = SerializerCore.Serialize(nspname)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(oid)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(typname)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(typtype)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(typnotnull)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(elemtypoid)});
  }
}
