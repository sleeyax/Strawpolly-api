using System.Collections.Generic;

namespace strawpoll.Api.Requests
{
    public class PollRequest
    {
        public long PollID { get; set; }
        public string Name { get; set; }
        public List<string> Answers { get; set; }
        // IDs of members that can participate in this poll
        public int[] participantIds { get; set; }
    }
}