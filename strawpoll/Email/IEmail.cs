namespace strawpoll.Email
{
    public interface IEmail
    {
        void Send(string fromAddress, string toAddress, string title, string body);
    }
}