using System.Linq;
using Microsoft.AspNetCore.Mvc;
using strawpoll.Api.Responses;
using strawpoll.Models;

namespace strawpoll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public StatsController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// retrieve statistics about the application
        /// </summary>
        /// <returns></returns>
        // GET: api/stats
        [HttpGet]
        public StatsResponse GetStats()
        {
            return new StatsResponse
            {
                MemberCount = _context.Members.Count(),
                PollCount = _context.Polls.Count()
            };
        }
    }
}
