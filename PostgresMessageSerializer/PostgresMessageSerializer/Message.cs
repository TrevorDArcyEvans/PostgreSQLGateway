namespace PostgresMessageSerializer;

public abstract class Message
{
  public abstract byte[] Serialize();
  public abstract void Deserialize(byte[] payload);
}
