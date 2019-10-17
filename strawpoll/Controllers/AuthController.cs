using System.Linq;
using Microsoft.AspNetCore.Mvc;
using strawpoll.Models;
using strawpoll.Services;

namespace strawpoll.Controllers
{
    public abstract class AuthController : ControllerBase
    {
        protected readonly DatabaseContext _context;

        protected AuthController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the member that's currently authenticated with the API
        /// </summary>
        /// <returns></returns>
        protected Member GetAuthenticatedMember()
        {
            string memberId = User.Claims.FirstOrDefault(c => c.Type == "MemberID")?.Value;
            return _context.Members.FirstOrDefault(m => m.MemberID.ToString() == memberId);
        }

    }
}