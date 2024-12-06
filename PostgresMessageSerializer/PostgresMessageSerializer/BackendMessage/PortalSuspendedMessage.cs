namespace PostgresMessageSerializer;

public class PortalSuspendedMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'s';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
