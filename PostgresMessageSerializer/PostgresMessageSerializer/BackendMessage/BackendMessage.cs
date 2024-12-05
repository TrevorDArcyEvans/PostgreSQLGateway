namespace PostgresMessageSerializer;

public abstract class BackendMessage
{
  public abstract void Deserialize(byte[] payload);
}
