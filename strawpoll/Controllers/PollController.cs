using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// list polls I have created
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// list polls I'm invited to
        /// </summary>
        /// <returns></returns>
        // GET: api/polls/open
        [Authorize]
        [HttpGet("open")]
        public async Task<List<PollResponse>> GetOpenPolls()
        {
            Member member = GetAuthenticatedMember();

            var result = await _context.Polls
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Where(poll => poll.Participants.Any(p => p.MemberID == member.MemberID))
                .Include(p => p.Answers).ThenInclude(a => a.Votes)
                .Include(p => p.Creator)
                .Select(p => ToPollResponse(p))
                .ToListAsync();

            return result.Select(r =>
            {
                r.Vote = GetPollVote(r.PollID, member);
                return r;
            }).ToList();
        }

        /// <summary>
        /// info about a poll I can participate in
        /// </summary>
        /// <param name="id">poll id</param>
        /// <returns></returns>
        // GET: api/polls/open/5
        [Authorize]
        [HttpGet("open/{id}")]
        public async Task<ActionResult<PollResponse>> GetOpenPoll(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = _context.Polls
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Include(p => p.Answers).ThenInclude(a => a.Votes)
                .Include(p => p.Creator)
                .FirstOrDefault(p => p.PollID == id && p.Participants.Any(part => part.MemberID == member.MemberID));

            if (poll == null)
                return NotFound();

            var response = ToPollResponse(poll);
            response.Vote = GetPollVote(poll.PollID, member);

            return response;
        }

        /// <summary>
        /// poll voting results
        /// </summary>
        /// <param name="id">poll id</param>
        /// <returns></returns>
        // GET: api/polls/results/5
        [Authorize]
        [HttpGet("results/{id}")]
        public async Task<ActionResult<PollResponse>> GetPollResults(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = _context.Polls
                .Include(p => p.Participants).ThenInclude(part => part.Member)
                .Include(p => p.Answers).ThenInclude(a => a.Votes)
                .Include(p => p.Creator)
                .FirstOrDefault(p => p.PollID == id && (p.Participants.Any(part => part.MemberID == member.MemberID) || p.Creator.MemberID == member.MemberID));

            if (poll == null)
                return PollNotFound();

            var response = ToPollResponse(poll);
            // include amount of votes for each answer
            response.Answers = poll.Answers.Select(a => new AnswerResponse
            {
                Answer = a.Answer,
                AnswerID = a.PollAnswerID,
                Votes = a.Votes.Count
            }).ToList();

            return response;
        }

        private VoteResponse GetPollVote(long pollId, Member member)
        {
            var pollVote = _context.PollVotes
                .Include(pv => pv.Answer)
                .FirstOrDefault(pv => pv.MemberID == member.MemberID && pv.Answer.PollID == pollId);

            return pollVote != null ? new VoteResponse
            {
                VoteID = pollVote.PollVoteID,
                AnswerID = pollVote.PollAnswerID
            } : null;
        }

        /// <summary>
        /// info about poll that I have created
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// map Poll to PollResponse
        /// </summary>
        /// <param name="poll"></param>
        /// <returns></returns>
        private PollResponse ToPollResponse(Poll poll)
        {
            return new PollResponse
            {
                PollID = poll.PollID,
                Answers = poll.Answers.Select(a => new AnswerResponse
                {
                    Answer = a.Answer,
                    AnswerID = a.PollAnswerID
                }).ToList(),
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
                    },
                }).ToList()
            };
        }

        /// <summary>
        /// update poll
        /// </summary>
        /// <param name="id">poll id</param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/polls/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoll(long id, PollRequest request)
        {
            if (id != request.PollID)
                return BadRequest();

            Member member = GetAuthenticatedMember();

            Poll poll = _context.Polls
                .Where(p => p.Creator.MemberID == member.MemberID)
                .Include(p => p.Answers)
                .Include(p => p.Participants)
                .FirstOrDefault(p => p.PollID == request.PollID);

            // poll doesn't exist or member has no permission to edit
            if (poll == null) return PollNotFound();

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

        /// <summary>
        /// create poll
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

            return Ok(poll);
        }

        /// <summary>
        /// delete poll
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/poll/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Poll>> DeletePoll(long id)
        {
            Member member = GetAuthenticatedMember();

            var poll = await _context.Polls.FindAsync(id);
            if (poll == null)
                return PollNotFound();

            // check if the member is allowed to delete this poll
            if (member.MemberID != poll.Creator.MemberID)
                return PollNotFound(); // NOTE: we do not use Unauthorized() here, because in that case the client will think the token is invalid

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PollExists(long id) => _context.Polls.Any(e => e.PollID == id);

        private NotFoundObjectResult PollNotFound() => NotFound("Poll not found!");
    }
}
