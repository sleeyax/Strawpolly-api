using strawpoll.Models;

namespace strawpoll.Api.Responses
{
    public class PollParticipantResponse
    {
        public long PollParticipantID { get; set; }
        public MemberResponse Participant { get; set; }
        // whether or not this participant has answered on a poll
        public bool HasAnswered { get; set; }
    }
}