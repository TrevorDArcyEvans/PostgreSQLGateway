namespace NpgsqlMessageHandler;

using PostgresMessageSerializer;

public class VersionInfoDescription : RowDescriptionMessage<VersionInfo>
{
  public VersionInfoDescription()
  {
    RowFieldDescriptions.Add(new RowFieldDescription(typeof(VersionInfo), nameof(VersionInfo.version)));
  }
}
