namespace PostgresMessageSerializer
{
    public class CancelRequestMessage : FrontendMessage
    {
        public int Length { get; } = 16;
        public int CancelRequestCode { get; } = 80877102;
        public int ProcessId { get; set; }
        public int SecretKey { get; set; }

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.Write(Length);
            buffer.Write(CancelRequestCode);
            buffer.Write(ProcessId);
            buffer.Write(SecretKey);

            return buffer.ToArray();
        }
    }
}