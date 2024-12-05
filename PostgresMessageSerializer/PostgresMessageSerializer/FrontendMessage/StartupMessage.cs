namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class StartupMessage : FrontendMessage
{
  public int ProtocolVersion { get; } = 196608; // 3, 0, 0, 0

  public IList<StartupParameter> Parameters { get; set; }

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
}
