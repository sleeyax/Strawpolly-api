using System.Collections.Generic;
using strawpoll.Models;

namespace strawpoll.Api.Responses
{
    public class PollResponse
    {
        public long PollID { get; set; }
        public string Name { get; set; }
        public MemberResponse Creator { get; set; }
        public VoteResponse Vote { get; set; }
        public List<AnswerResponse> Answers { get; set; }
        public List<PollParticipantResponse> Participants { get; set; }
    }
}