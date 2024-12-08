namespace PostgresMessageSerializer;

public class ParameterStatusMessage : BackendMessage
{
  public override byte MessageTypeId => (byte) 'S';

  public string Name { get; set; } = string.Empty;

  public string Value { get; set; } = string.Empty;

  public ParameterStatusMessage()
  {
  }

  public ParameterStatusMessage(string name, string value) :
    base()
  {
    Name = name;
    Value = value;
  }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(Name);
    buffer.Write(Value);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    Name = buffer.ReadString();
    Value = buffer.ReadString();
  }
}
