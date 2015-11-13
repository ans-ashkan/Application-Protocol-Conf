namespace Common
{
    public class RejectSession:MessageBase
    {
        public RejectSession()
        {
            MessageType = MessageType.RejectSession;
        }

        public string Reason { get; set; }
    }
}