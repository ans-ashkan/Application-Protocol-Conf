namespace Common
{
    public class AcceptSession:MessageBase
    {
        public AcceptSession()
        {
            MessageType = MessageType.AcceptSession;
        }

        public string WellcomeMessage { get; set; }
        public long SessionId { get; set; }
    }
}