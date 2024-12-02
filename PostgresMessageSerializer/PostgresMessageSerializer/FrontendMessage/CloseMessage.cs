namespace PostgresMessageSerializer
{
    public class CloseMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'X';

        public override byte[] Serialize()
        {
            return new byte[] { };
        }
    }
}