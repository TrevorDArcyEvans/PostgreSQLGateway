namespace PostgresMessageSerializer;

public class SSLRequestMessage : FrontendMessage
{
  public override byte MessageTypeId => 2;

  /// <summary>
  /// The SSL request code.
  /// The value is chosen to contain 1234 in the most significant 16 bits, and 5679 in the least significant 16 bits.
  /// (To avoid confusion, this code must not be the same as any protocol version number.)
  /// </summary>
  public static int SSLRequestMessageId { get; } = 80877103;

  public override byte[] Serialize()
  {
    return [];
  }

  public override void Deserialize(byte[] payload)
  {
  }
}
