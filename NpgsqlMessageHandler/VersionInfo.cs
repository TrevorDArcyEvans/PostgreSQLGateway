namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

/// <summary>
/// Data from:
///     _select_version__202412151616.csv
/// </summary>
public class VersionInfo : DataRowMessage
{
  public string version { get; set; } // 25 = TEXT
}
