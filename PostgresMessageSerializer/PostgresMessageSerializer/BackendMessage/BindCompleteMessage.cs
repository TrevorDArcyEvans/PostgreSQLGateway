namespace PostgresMessageSerializer
{
    public class BindCompleteMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'2';

        public override void Deserialize(byte[] payload)
        { }
    }
}
