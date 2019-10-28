using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Api.Requests;
using strawpoll.Api.Responses;
using strawpoll.Models;

namespace strawpoll.Controllers
{
    [Route("api/polls")]
    [ApiController]
    public class PollController : AuthController
    {
        public PollController(DatabaseContext context) : base(context) { }

        // Returns all polls the current user has created
        // GET: api/polls
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<PollResponse>> GetMemberPolls()
        {
            Member member = GetAuthenticatedMember();

            return await _context.Polls
                .Where(p => p.Creator == member)
                .Include(p => p.Answers)
                .Include(p => p.Creator)
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Select(p => ToPollResponse(p))
                .ToListAsync();
        }

        // Returns all polls the current member is invited to
        // GET: api/polls/open
        [Authorize]
        [HttpGet("open")]
        public async Task<IEnumerable<PollResponse>> GetOpenPolls()
        {
            Member member = GetAuthenticatedMember();

           // return _context.PollParticipants.Where(pp => pp.MemberID == member.MemberID).Include(pp => pp.Poll).Select(pp => ToPollResponse(pp));

            return await _context.Polls
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Where(poll => poll.Participants.Any(p => p.MemberID == member.MemberID))
                .Include(p => p.Answers)
                .Include(p => p.Creator)
                .Select(p => ToPollResponse(p))
                .ToListAsync();
        }

        // GET: api/polls/open/5
        [Authorize]
        [HttpGet("open/{id}")]
        public async Task<ActionResult<PollResponse>> GetOpenPoll(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = _context.Polls
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Include(p => p.Answers)
                .Include(p => p.Creator)
                .FirstOrDefault(p => p.PollID == id && p.Participants.Any(part => part.MemberID == member.MemberID));

            if (poll == null)
                return NotFound();

            return ToPollResponse(poll);
        }

        // GET: api/polls/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PollResponse>> GetPoll(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = _context.Polls
                .Include(p => p.Answers)
                .Include(p => p.Creator)
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .FirstOrDefault(p => p.Creator == member && p.PollID == id);

            if (poll == null)
                return NotFound();

            return ToPollResponse(poll);
        }

        private PollResponse ToPollResponse(Poll poll)
        {
            return new PollResponse
            {
                PollID = poll.PollID,
                Answers = poll.Answers.Select(a => new AnswerResponse {Answer = a.Answer, AnswerID = a.PollAnswerID}).ToList(),
                Creator = new MemberResponse
                {
                    MemberID = poll.Creator.MemberID,
                    Email = poll.Creator.Email,
                    FirstName = poll.Creator.FirstName,
                    LastName = poll.Creator.LastName
                },
                Name = poll.Name,
                Participants = poll.Participants.Select(p => new PollParticipantResponse
                {
                    PollParticipantID = p.PollParticipantID,
                    Participant = new MemberResponse
                    {
                        Email = p.Member.Email,
                        FirstName = p.Member.FirstName,
                        LastName = p.Member.LastName,
                        MemberID = p.Member.MemberID,
                    }
                }).ToList()
            };
        }

        // PUT: api/polls/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoll(long id, PollRequest request)
        {
            if (id != request.PollID)
                return BadRequest( id + " " + request.PollID);

            Member member = GetAuthenticatedMember();

            Poll poll = _context.Polls
                .Where(p => p.Creator.MemberID == member.MemberID)
                .Include(p => p.Answers)
                .Include(p => p.Participants)
                .FirstOrDefault(p => p.PollID == request.PollID);

            if (poll == null) return NotFound();

            // TODO: check if creating new object is actually securer
            poll.Answers = request.Answers.Select(a => new PollAnswer {Answer = a.Answer}).ToList();
            poll.Name = request.Name;
            poll.Participants = request.Participants.Select(p => new PollParticipant {MemberID = p.MemberID}).ToList();

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

            return Ok(poll);
        }

        // POST: api/polls
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostPoll(PollRequest request)
        {
            Member member = GetAuthenticatedMember();

            Poll poll = new Poll
            {
                Creator = member,
                Answers = request.Answers
                    .Select(a => new PollAnswer{Answer = a.Answer})
                    .ToList(),
                Name = request.Name,
                Participants = request.Participants.Select(p => new PollParticipant() {MemberID = p.MemberID}).ToList()
            };

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPoll", new { id = poll.PollID }, poll);
        }

        // DELETE: api/poll/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Poll>> DeletePoll(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = await _context.Polls.FindAsync(id);
            if (poll == null)
                return NotFound();

            // check if the member is allowed to delete this poll
            if (member.MemberID != poll.Creator.MemberID)
                return Unauthorized();

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
