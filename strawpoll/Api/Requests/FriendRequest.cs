using System.Collections.Generic;

namespace strawpoll.Api.Requests
{
    public class FriendRequest
    {
        public long MemberID { get; set; }
        public List<string> FriendEmails { get; set; }
    }
}