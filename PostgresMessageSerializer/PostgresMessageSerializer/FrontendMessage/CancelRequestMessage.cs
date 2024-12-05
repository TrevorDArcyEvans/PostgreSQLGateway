namespace PostgresMessageSerializer;

using System.Runtime.Serialization;

public class CancelRequestMessage : FrontendMessage
{
  public int Length { get; } = 16;
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
