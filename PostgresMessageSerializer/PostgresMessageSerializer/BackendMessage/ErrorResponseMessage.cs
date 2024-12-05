namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class ErrorResponseMessage : BackendMessage
{
  public static byte MessageTypeId = (byte) 'E';

  public IList<MessageField> Fields { get; } = new List<MessageField>();

  public override byte[] Serialize()
  {
    using var buffer = new PostgresProtocolStream();

    for (var i = 0; i < Fields.Count; i++)
    {
      var msgField = Fields[i];
      buffer.Write(msgField.Id);
      buffer.Write(msgField.Value);
    }
    buffer.Write('\0');

    return buffer.ToArray();
  }

  public override void Deserialize(byte[] payload)
  {
    using var buffer = new PostgresProtocolStream(payload);

    while (true)
    {
      var fieldId = (byte) buffer.ReadByte();
      if (fieldId == '\0')
      {
        break;
      }

      var messageField = new MessageField();
      messageField.Id = fieldId;
      messageField.Value = buffer.ReadString();
      Fields.Add(messageField);
    }
  }
}
