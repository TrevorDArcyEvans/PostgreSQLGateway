namespace PostgresMessageSerializer;

public abstract class Message
{
  public abstract byte MessageTypeId { get; }

  public abstract byte[] Serialize();
  public abstract void Deserialize(byte[] payload);
}
