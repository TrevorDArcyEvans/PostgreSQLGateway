namespace PostgresMessageSerializer
{
    public class SyncMessage : FrontendMessage
    {
        public static byte MessageTypeId = (byte)'S';

        public override byte[] Serialize()
        {
            return new byte[] { };
        }
    }
}