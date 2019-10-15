using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Models;

namespace strawpoll.Controllers
{
    [Route("api/polls")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PollController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the member that's currently authenticated with the API
        /// </summary>
        /// <returns></returns>
        private Member getCurrentMember()
        {
            string memberId = User.Claims.FirstOrDefault(c => c.Type == "MemberID")?.Value;
            return _context.Members.FirstOrDefault(m => m.MemberID.ToString() == memberId);
        }

        // Returns all polls the current user has created
        // GET: api/polls
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poll>>> GetPolls()
        {
            Member member = getCurrentMember();

            return await _context.Polls
                .Where(p => p.Creator == member)
                .Include(p => p.Answers)
                .ToListAsync();
        }

        // GET: api/polls/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Poll>> GetPoll(long id)
        {
            Member member = getCurrentMember();

            var poll = _context.Polls.FirstOrDefault(p => p.Creator == member && p.PollID == id);

            if (poll == null)
                return NotFound();
            

            return poll;
        }

        // PUT: api/polls/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoll(long id, Poll poll)
        {
            if (id != poll.PollID)
            {
                return BadRequest();
            }

            _context.Entry(poll).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/polls
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> PostPoll(Poll poll)
        {
            Member member = getCurrentMember();
            poll.Creator = member;

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            return new JsonResult(new JsonResponse(Status.SUCCESS, "Poll created successfully"));
        }

        // DELETE: api/poll/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Poll>> DeletePoll(long id)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll == null)
            {
                return NotFound();
            }

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();

            return poll;
        }

        private bool PollExists(long id)
        {
            return _context.Polls.Any(e => e.PollID == id);
        }
    }
}
