namespace PostgresMessageSerializer
{
    public class CloseCompleteMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'3';

        public override void Deserialize(byte[] payload)
        { }
    }
}