using strawpoll.Models;

namespace strawpoll.Api.Responses
{
    public class FriendResponse
    {
        public long FriendID { get; set; }
        public FriendStatus FriendStatus { get; set; }
        public Member Friend { get; set; }
    }
}