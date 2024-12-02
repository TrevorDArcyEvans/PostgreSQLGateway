namespace PostgresMessageSerializer
{
    public class CommandCompleteMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'C';

        public string CommandTag { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            CommandTag = buffer.ReadString();
        }
    }
}