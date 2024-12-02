namespace PostgresMessageSerializer
{
    public abstract class FrontendMessage
    {
        public abstract byte[] Serialize();
    }
}