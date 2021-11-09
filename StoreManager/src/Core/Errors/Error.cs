namespace Core.Errors
{
    public class Error
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public Error(int statusCodes, string message)
        {
            StatusCode = statusCodes;
            Message = message;
        }

        public Error()
        {
            
        }
    }
}