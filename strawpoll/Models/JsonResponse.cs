namespace strawpoll.Models
{
    public class Status
    {
        public const string SUCCESS = "success";
        public const string ERROR = "error";
    }

    public class JsonResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }

        public JsonResponse(string status, string message, object data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}