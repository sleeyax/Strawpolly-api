using strawpoll.Models;

namespace strawpoll.Api.Requests
{
    public class FriendStatusUpdateRequest
    {
        public long FriendID { get; set; }
        public FriendStatus FriendStatus { get; set; }
    }
}