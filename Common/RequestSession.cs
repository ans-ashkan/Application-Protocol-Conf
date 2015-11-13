namespace Common
{
    public class RequestSession : MessageBase
    {
        public RequestSession()
        {
            MessageType = MessageType.RequestSession;
            Version = 1;
        }

        public int Version { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}