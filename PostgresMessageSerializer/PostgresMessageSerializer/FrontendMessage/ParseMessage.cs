namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class ParseMessage : FrontendMessage
{
  public static byte MessageTypeId = (byte)'P';

  public string PreparedStatementName { get; set; } = string.Empty;

  public string Query { get; set; } = string.Empty;

  public short ParameterDataTypeOidsCount => (short)ParameterDataTypeOids.Count;

  public IList<int> ParameterDataTypeOids { get; set; } = new List<int>();

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(PreparedStatementName);
    buffer.Write(Query);
    buffer.Write(ParameterDataTypeOidsCount);

    for (var i = 0; i < ParameterDataTypeOidsCount; i++)
    {
      buffer.Write(ParameterDataTypeOids[i]);
    }

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);
    throw new System.NotImplementedException();
  }
}
