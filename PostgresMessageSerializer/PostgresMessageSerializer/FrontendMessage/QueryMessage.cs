namespace PostgresMessageSerializer;

public class QueryMessage : FrontendMessage
{
  public static byte MessageTypeId = (byte)'Q';

  public string Query { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(Query);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);
    
    Query = buffer.ReadString();
  }
}
