namespace PostgresMessageSerializer
{
    public class AuthenticationMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'R';

        public int AuthResult { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            AuthResult = buffer.ReadInt32();
        }
    }
}
