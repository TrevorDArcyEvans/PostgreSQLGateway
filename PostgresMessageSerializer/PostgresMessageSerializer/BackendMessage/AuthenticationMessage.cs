namespace PostgresMessageSerializer;

public class AuthenticationMessage : BackendMessage
{
  public override byte MessageTypeId => (byte)'R';

  public int AuthResult { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(AuthResult);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    AuthResult = buffer.ReadInt32();
  }
}
