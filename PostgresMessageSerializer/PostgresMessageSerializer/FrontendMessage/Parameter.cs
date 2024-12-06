namespace PostgresMessageSerializer;

public class Parameter
{
  public int Length => Value.Length;

  public byte[] Value { get; set; } = [];
}
