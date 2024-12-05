namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class StartupMessage : FrontendMessage
{
  public int ProtocolVersion { get; private set; } = 196608; // 3, 0, 0, 0

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

  public override void Deserialize(byte[] buffer)
  {
    using var strm = new PostgresProtocolStream(buffer);
    ProtocolVersion = strm.ReadInt32();

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
