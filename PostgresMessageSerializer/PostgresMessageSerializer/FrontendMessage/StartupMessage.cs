namespace PostgresMessageSerializer;

using System.Collections.Generic;
using System.Runtime.Serialization;

public class StartupMessage : FrontendMessage
{
  public int ProtocolVersion { get; } = 196608; // 3, 0, 0, 0

  public IList<StartupParameter> Parameters { get; } = new List<StartupParameter>();

  public byte EndMessage { get; } = (byte)0;

  public override byte[] Serialize()
  {
    var buffer = new PostgresProtocolStream();

    buffer.Write(ProtocolVersion);

    for (var i = 0; i < Parameters.Count; i++)
    {
      buffer.Write(Parameters[i].Name);
      buffer.Write(Parameters[i].Value);
    }

    buffer.WriteByte(EndMessage);

    return buffer.ToArray();
  }

  public static StartupMessage Deserialize(byte[] buffer)
  {
    var retval = new StartupMessage();
    var strm = new PostgresProtocolStream(buffer);
    var protVer = strm.ReadInt32();

    if (protVer != retval.ProtocolVersion)
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

      retval.Parameters.Add(new StartupParameter(name, value));
    }

    return retval;
  }
}
