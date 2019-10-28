using System.Collections.Generic;
using strawpoll.Models;

namespace strawpoll.Api.Requests
{
    public class PollRequest
    {
        public long PollID { get; set; }
        public string Name { get; set; }
        public List<PollAnswer> Answers { get; set; }
        // IDs of members that can participate in this poll
        public List<PollParticipant> Participants { get; set; }
    }
}