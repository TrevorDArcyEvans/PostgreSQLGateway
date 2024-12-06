namespace PostgresMessageSerializer;

using System.Collections.Generic;
using System.Runtime.Serialization;

public class StartupMessage : FrontendMessage
{
  public override byte MessageTypeId => 1;

  public static int ProtocolVersion { get; } = 196608; // 0x30000

  public IList<StartupParameter> Parameters { get; } = new List<StartupParameter>();

  private byte EndMessage { get; } = (byte)0;

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(ProtocolVersion);

    for (var i = 0; i < Parameters.Count; i++)
    {
      buffer.Write(Parameters[i].Name);
      buffer.Write(Parameters[i].Value);
    }

    buffer.WriteByte(EndMessage);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] buffer)
  {
    using var strm = new PostgresProtocolStream(buffer);

    var protVer = strm.ReadInt32();
    if (protVer != ProtocolVersion)
    {
      throw new InvalidDataContractException($"Invalid protocol version: {protVer}");
    }

    while (true)
    {
      var name = strm.ReadString();

      if (string.IsNullOrEmpty(name))
      {
        break;
      }

      var value = strm.ReadString();

      Parameters.Add(new StartupParameter(name, value));
    }
  }
}
