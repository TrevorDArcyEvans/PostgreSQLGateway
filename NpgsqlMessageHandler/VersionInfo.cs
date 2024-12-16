namespace NpgsqlMessageHandler;

using System.Reflection;
using System.Globalization;
using CsvHelper;
using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _select_version__202412151616.csv
/// </summary>
public class VersionInfo : DataRowMessage
{
  public static readonly VersionInfo Instance;

  static VersionInfo()
  {
    using var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_select_version__202412151616.csv"));
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    var records = csv.GetRecords<VersionInfo>().ToList();

    Instance = records.Single();
  }

  private string _version;

  public string version
  {
    get => _version;

    set
    {
      _version = value;

      Rows.Clear();
      var versionField = new RowField
      {
        Value = SerializerCore.Serialize(version)
      };
      Rows.Add(versionField);
    }
  }

  private VersionInfo()
  {
  }
}
