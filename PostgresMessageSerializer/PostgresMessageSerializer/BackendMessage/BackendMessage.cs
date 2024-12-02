namespace PostgresMessageSerializer
{
    public abstract class BackendMessage
    {
        public abstract void Deserialize(byte[] payload);
    }

    public class MessageField
    {
        public byte Id { get; set; }
        public string Value { get; set; }
    }
}
