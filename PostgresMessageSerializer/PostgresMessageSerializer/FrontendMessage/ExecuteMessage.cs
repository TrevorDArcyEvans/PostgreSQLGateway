namespace PostgresMessageSerializer;

public class ExecuteMessage : FrontendMessage
{
  public override byte MessageTypeId => (byte) 'E';

  public string PortalName { get; set; } = string.Empty;

  public int Limit { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(PortalName);
    buffer.Write(Limit);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    PortalName = buffer.ReadString();
    Limit = buffer.ReadInt32();
  }
}
