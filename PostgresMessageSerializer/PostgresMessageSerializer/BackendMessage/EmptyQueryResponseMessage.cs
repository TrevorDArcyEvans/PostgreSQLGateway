namespace PostgresMessageSerializer;

public class EmptyQueryResponseMessage : BackendMessage
{
  public static byte MessageTypeId = (byte)'I';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
