namespace PostgresMessageSerializer;

public class SyncMessage : FrontendMessage
{
  public override byte MessageTypeId => (byte)'S';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
