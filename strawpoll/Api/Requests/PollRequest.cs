using System.Collections.Generic;
using strawpoll.Models;

namespace strawpoll.Api.Requests
{
    public class AnswerRequest
    {
        public string Answer { get; set; }
    }

    public class PollRequest
    {
        public long PollID { get; set; }
        public string Name { get; set; }
        public List<AnswerRequest> Answers { get; set; }
    }
}