namespace strawpoll.Api.Responses
{
    public class MemberResponse
    {
        public long MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}