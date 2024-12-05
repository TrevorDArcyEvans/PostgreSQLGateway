namespace PostgresMessageSerializer;

using System.Collections.Generic;

public class NoticeResponseMessage : BackendMessage
{
  public static byte MessageTypeId = (byte)'N';

  public IList<MessageField> Fields { get; set; }

  public override byte[] Serialize()
  {
    throw new System.NotImplementedException();
  }

  public override void Deserialize(byte[] payload)
  {
    var buffer = new PostgresProtocolStream(payload);

    Fields = new List<MessageField>();

    while (true)
    {
      var fieldId = (byte)buffer.ReadByte();
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
