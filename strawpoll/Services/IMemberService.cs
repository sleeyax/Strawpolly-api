using strawpoll.Models;

namespace strawpoll.Services
{
    public interface IMemberService
    {
        Member Authenticate(string email, string password);
    }
}