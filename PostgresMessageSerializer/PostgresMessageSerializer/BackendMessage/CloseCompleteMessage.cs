namespace PostgresMessageSerializer;

public class CloseCompleteMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'3';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
