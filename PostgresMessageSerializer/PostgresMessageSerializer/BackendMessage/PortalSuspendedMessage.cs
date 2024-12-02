namespace PostgresMessageSerializer
{
    public class PortalSuspendedMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'s';

        public override void Deserialize(byte[] payload)
        { }
    }
}