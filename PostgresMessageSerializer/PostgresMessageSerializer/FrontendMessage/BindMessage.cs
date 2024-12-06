namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class BindMessage : FrontendMessage
{
  public static byte MessageTypeId = (byte)'B';

  public string PortalName { get; set; } = string.Empty;

  public string PreparedStatementName { get; set; } = string.Empty;

  public short ParameterFormatCodeCount => (short)ParameterFormatCodes.Count;

  public IList<short> ParameterFormatCodes { get; } = new List<short>(0);

  public short ParameterValueCount => (short)ParameterValues.Count;

  public IList<Parameter> ParameterValues { get; } = new List<Parameter>();

  public short RowFormatCode { get; set; } = 0;

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    buffer.Write(PortalName);
    buffer.Write(PreparedStatementName);
    buffer.Write(ParameterFormatCodeCount);

    for (var i = 0; i < ParameterFormatCodeCount; i++)
    {
      buffer.Write(ParameterFormatCodes[i]);
    }

    buffer.Write(ParameterValueCount);

    for (var i = 0; i < ParameterValueCount; i++)
    {
      buffer.Write(ParameterValues[i].Length);
      buffer.Write(ParameterValues[i].Value);
    }

    buffer.Write(RowFormatCode);

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    PortalName = buffer.ReadString();
    PreparedStatementName = buffer.ReadString();

    var paramFmtCodes = buffer.ReadInt16();
    for (var i = 0; i < paramFmtCodes; i++)
    {
      ParameterFormatCodes.Add(buffer.ReadInt16());
    }

    var paramVals = buffer.ReadInt16();
    for (var i = 0; i < paramVals; i++)
    {
      var length = buffer.ReadInt32();
      var values = new List<byte>(length);
      for (var j = 0; j < length; j++)
      {
        values.Add((byte)buffer.ReadByte());
      }

      var param = new Parameter
      {
        Value = values.ToArray()
      };
      ParameterValues.Add(param);
    }
  }
}
