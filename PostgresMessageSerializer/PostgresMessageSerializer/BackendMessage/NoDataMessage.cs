namespace PostgresMessageSerializer;

public class NoDataMessage : BackendMessage
{
  public static byte MessageTypeId = (byte)'n';

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
