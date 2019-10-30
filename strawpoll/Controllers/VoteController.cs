using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Api.Requests;
using strawpoll.Models;

namespace strawpoll.Controllers
{
    [Route("api/vote")]
    [ApiController]
    public class VoteController : AuthController
    {
        public VoteController(DatabaseContext context) : base(context) {}

        // GET: api/vote
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PollVote>>> GetPollVotes()
        {
            return await _context.PollVotes.ToListAsync();
        }

        // GET: api/vote/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PollVote>> GetPollVote(long id)
        {
            var pollVote = await _context.PollVotes.FindAsync(id);

            if (pollVote == null)
            {
                return NotFound();
            }

            return pollVote;
        }

        // PUT: api/vote/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPollVote(long id, PollVote pollVote)
        {
            if (id != pollVote.PollVoteID)
            {
                return BadRequest();
            }

            _context.Entry(pollVote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollVoteExists(id))
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

        // POST: api/vote
        [HttpPost]
        public async Task<ActionResult<PollVote>> PostPollVote(VoteRequest request)
        {
            // TODO: check if already voted

            Member member = GetAuthenticatedMember();

            PollParticipant pollParticipant = _context.PollParticipants
                .FirstOrDefault(p => p.MemberID == member.MemberID && p.PollID == request.PollID);

            if (pollParticipant == null)
                return NotFound();

            _context.PollVotes.Add(new PollVote
            {
                MemberID = member.MemberID,
                PollAnswerID = request.AnswerID
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/vote/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PollVote>> DeletePollVote(long id)
        {
            var pollVote = await _context.PollVotes.FindAsync(id);
            if (pollVote == null)
            {
                return NotFound();
            }

            _context.PollVotes.Remove(pollVote);
            await _context.SaveChangesAsync();

            return pollVote;
        }

        private bool PollVoteExists(long id)
        {
            return _context.PollVotes.Any(e => e.PollVoteID == id);
        }
    }
}
