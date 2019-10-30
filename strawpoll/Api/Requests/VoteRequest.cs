namespace strawpoll.Api.Requests
{
    public class VoteRequest
    {
        public long PollID { get; set; }
        public long AnswerID { get; set; }
    }
}