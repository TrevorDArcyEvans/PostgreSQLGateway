namespace PostgresMessageSerializer;

public class TerminateMessage : FrontendMessage
{
  public override byte MessageTypeId => (byte)'X';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
