namespace Common
{
    public class Error:MessageBase
    {
        public Error()
        {
            MessageType = MessageType.Error;
        }

        public string ErrorMessage { get; set; }
    }
}