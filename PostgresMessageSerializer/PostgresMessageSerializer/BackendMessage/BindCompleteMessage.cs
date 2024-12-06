namespace PostgresMessageSerializer;

public class BindCompleteMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'2';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
