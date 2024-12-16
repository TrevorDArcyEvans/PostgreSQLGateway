namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _select_version__202412151616.csv
/// </summary>
public class VersionInfo : DataRowMessage
{
  public string version { get; } // 25 = TEXT

  public VersionInfo(string version)
  {
    this.version = version;
    var versionField = new RowField
    {
      Value = SerializerCore.Serialize(this.version)
    };
    Rows.Add(versionField);
  }
}
