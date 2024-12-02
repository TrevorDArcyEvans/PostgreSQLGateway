namespace PostgresMessageSerializer
{
    public class ReadyForQueryMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'Z';

        public byte TransactionStatus { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            TransactionStatus = (byte)buffer.ReadByte();
        }
    }
}