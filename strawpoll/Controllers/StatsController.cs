using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
