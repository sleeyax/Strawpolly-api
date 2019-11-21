namespace strawpoll.Api.Responses
{
    public class AnswerResponse
    {
        public long AnswerID { get; set; }
        public string Answer { get; set; }
        public int? Votes { get; set; }
    }
}