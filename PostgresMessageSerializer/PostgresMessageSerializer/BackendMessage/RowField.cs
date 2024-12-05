namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class RowField
{
  public int Length { get; set; }

  public IList<byte> Value { get; set; }
}
