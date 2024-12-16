namespace NpgsqlMessageHandler;

using System.Globalization;
using System.Reflection;
using CsvHelper;
using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _Load_enum_fields_SELECT_pg_type_oid_enumlabel_FROM_pg_enum_JOIN_202412151617.csv
/// </summary>
public class EnumField : DataRowMessage
{
  public static readonly Lazy<List<EnumField>> Instance = new(() =>
  {
    const string DataFile = "_Load_enum_fields_SELECT_pg_type_oid_enumlabel_FROM_pg_enum_JOIN_202412151617.csv";

    using var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DataFile));
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    return csv.GetRecords<EnumField>().ToList();
  });

  public int oid { get; set; } // 26 = OID
  public string enumlabel { get; set; } // 19 = NAME

  public override void Update()
  {
    Rows.Clear();

    Rows.Add(new RowField {Value = SerializerCore.Serialize(oid)});
    Rows.Add(new RowField {Value = SerializerCore.Serialize(enumlabel)});
  }
}
