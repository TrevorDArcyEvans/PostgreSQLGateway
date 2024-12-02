namespace PostgresMessageSerializer
{
    public class QueryMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'Q';

        public string Query { get; set; }

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.Write(Query);

            return buffer.ToArray();
        }
    }
}