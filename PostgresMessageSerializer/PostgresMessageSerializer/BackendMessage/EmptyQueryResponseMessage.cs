namespace PostgresMessageSerializer
{
    public class EmptyQueryResponseMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'I';

        public override void Deserialize(byte[] payload)
        { }
    }
}