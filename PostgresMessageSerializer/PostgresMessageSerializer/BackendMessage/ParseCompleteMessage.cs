namespace PostgresMessageSerializer
{
    public class ParseCompleteMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'1';

        public override void Deserialize(byte[] payload)
        { }
    }
}