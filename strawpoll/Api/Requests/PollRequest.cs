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
        // TODO: just use a list of strings here
        public List<AnswerRequest> Answers { get; set; }
        // IDs of members that can participate in this poll
        public int[] participantIds { get; set; }
    }
}