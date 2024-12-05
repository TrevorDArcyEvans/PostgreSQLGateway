namespace PostgresMessageSerializer;

public class BackendKeyDataMessage : BackendMessage
{
  public static byte MessageTypeId = (byte)'K';

  public int ProcessId { get; set; }

  public int SecretKey { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(ProcessId);
    buffer.Write(SecretKey);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    var buffer = new PostgresProtocolStream(payload);

    ProcessId = buffer.ReadInt32();
    SecretKey = buffer.ReadInt32();
  }
}
