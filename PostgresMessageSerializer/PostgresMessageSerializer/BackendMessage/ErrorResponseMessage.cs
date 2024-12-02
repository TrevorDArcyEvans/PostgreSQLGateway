using System.Collections.Generic;

namespace PostgresMessageSerializer
{
    public class ErrorResponseMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'E';

        public IList<MessageField> Fields { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            Fields = new List<MessageField>();

            while (true)
            {
                var fieldId = (byte)buffer.ReadByte();
                if (fieldId == '\0') break;

                var messageField = new MessageField();
                messageField.Id = fieldId;
                messageField.Value = buffer.ReadString();
                Fields.Add(messageField);
            }
        }
    }
}