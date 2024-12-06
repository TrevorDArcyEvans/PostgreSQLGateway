namespace PostgresMessageSerializer;

public class ParseCompleteMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'1';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
