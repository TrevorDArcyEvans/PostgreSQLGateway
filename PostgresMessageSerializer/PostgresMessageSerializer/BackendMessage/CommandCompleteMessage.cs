namespace PostgresMessageSerializer;

public class CommandCompleteMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'C';

  public string CommandTag { get; set; } = string.Empty;

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(CommandTag);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    CommandTag = buffer.ReadString();
  }
}
