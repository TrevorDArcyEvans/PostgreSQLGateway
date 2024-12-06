namespace PostgresMessageSerializer;

using System.Runtime.Serialization;

public class CancelRequestMessage : FrontendMessage
{
  public override byte MessageTypeId => 0;

  public int Length { get; } = 16;

  /// <summary>
  /// The cancel request code.
  /// The value is chosen to contain 1234 in the most significant 16 bits, and 5678 in the least significant 16 bits.
  /// (To avoid confusion, this code must not be the same as any protocol version number.)
  /// </summary>
  public int CancelRequestCode { get; } = 80877102;

  public int ProcessId { get; set; }
  public int SecretKey { get; set; }

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(Length);
    buffer.Write(CancelRequestCode);
    buffer.Write(ProcessId);
    buffer.Write(SecretKey);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    if (Length != buffer.ReadInt32())
    {
      throw new InvalidDataContractException("Invalid length");
    }

    if (CancelRequestCode != buffer.ReadInt32())
    {
      throw new InvalidDataContractException("Invalid cancel request code");
    }

    ProcessId = buffer.ReadInt32();
    SecretKey = buffer.ReadInt32();
  }
}
