namespace Common
{
    public class Broadcast : MessageBase
    {
        public Broadcast()
        {
            MessageType = MessageType.Broadcast;
        }

        public long SessionId { get; set; }
        public string Message { get; set; }
    }
}