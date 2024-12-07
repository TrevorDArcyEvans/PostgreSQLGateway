namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class RowField
{
  public int Length => Value.Count;

  public IList<byte> Value { get; set; } = new List<byte>();
}
