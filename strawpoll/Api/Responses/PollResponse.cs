using System.Collections.Generic;
using strawpoll.Models;

namespace strawpoll.Api.Responses
{
    public class PollResponse
    {
        public long PollID { get; set; }
        public string Name { get; set; }
        public MemberResponse Creator { get; set; }
        public List<string> Answers { get; set; }
        public List<long> ParticipantIds { get; set; }
    }
}