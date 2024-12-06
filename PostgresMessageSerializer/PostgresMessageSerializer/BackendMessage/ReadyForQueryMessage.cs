namespace PostgresMessageSerializer;

public class ReadyForQueryMessage : BackendMessage
{
  public override byte MessageTypeId => (byte) 'Z';

  public byte TransactionStatus { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();
    
    buffer.Write(TransactionStatus);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    TransactionStatus = (byte) buffer.ReadByte();
  }
}
