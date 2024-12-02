namespace PostgresMessageSerializer
{
    public class DescribeMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'D';

        public byte TargetType { get; set; }

        public string TargetName { get; set; } = string.Empty;

        public override byte[] Serialize()
        {
            var buffer = new PostgresProtocolStream();

            buffer.WriteByte(TargetType);
            buffer.Write(TargetName);

            return buffer.ToArray();
        }
    }
}