namespace PostgresMessageSerializer
{
    public class ExecuteMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'E';

        public string PortalName { get; set; } = string.Empty;

        public int Limit { get; set; }

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.Write(PortalName);
            buffer.Write(Limit);

            return buffer.ToArray();
        }
    }
}