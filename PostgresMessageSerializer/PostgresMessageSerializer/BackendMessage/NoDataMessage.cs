namespace PostgresMessageSerializer
{
    public class NoDataMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'n';

        public override void Deserialize(byte[] payload)
        { }
    }
}