namespace PostgresMessageSerializer;

public class Parameter
{
  public int Length
  {
    get
    {
      return Value.Length;
    }
  }

  public byte[] Value { get; set; }
}
