namespace PostgresMessageSerializer
{
    public class ParameterStatusMessage : BackendMessage
    {
        public static byte MessageTypeId = (byte)'S';

        public string Name { get; set; }

        public string Value { get; set; }

        public override void Deserialize(byte[] payload)
        {
            var buffer = new PostgresProtocolStream(payload);

            Name = buffer.ReadString();
            Value = buffer.ReadString();
        }
    }
}