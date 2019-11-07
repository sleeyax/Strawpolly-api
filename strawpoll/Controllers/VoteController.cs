﻿using System;
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
        /*[HttpGet]
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
        }*/

        // PUT: api/vote
        // update vote
        [HttpPut]
        public async Task<ActionResult<PollVote>> PutPollVote(VoteRequest request)
        {
            Member member = GetAuthenticatedMember();

            if (!IsParticipant(request.PollID, member)) return NotFound();

            // if for some reason the user hasn't voted yet, submit it as a new vote
            if (!HasVoted(request.PollID, member)) return await PostPollVote(request);

            // change answer
            var pollVote = _context.PollVotes.FirstOrDefault(pv => pv.PollVoteID == request.VoteID && pv.MemberID == member.MemberID);
            if (pollVote == null) return BadRequest("Invalid action for this user");
            pollVote.PollAnswerID = request.AnswerID;
            _context.Entry(pollVote).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/vote
        [HttpPost]
        public async Task<ActionResult<PollVote>> PostPollVote(VoteRequest request)
        {
            Member member = GetAuthenticatedMember();

            // check if member is a participant of this poll
            if (!IsParticipant(request.PollID, member)) return NotFound();

            // check if member has already voted
            if (HasVoted(request.PollID, member)) return BadRequest("Already voted!");

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

        /// <summary>
        /// check if member is a participant of a poll
        /// </summary>
        /// <param name="pollId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool IsParticipant(long pollId, Member member)
        {
            PollParticipant pollParticipant = _context.PollParticipants.FirstOrDefault(p => p.MemberID == member.MemberID && p.PollID == pollId);
            return pollParticipant != null;
        }

        /// <summary>
        /// verify if the member has voted on the poll
        /// </summary>
        /// <param name="pollId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool HasVoted(long pollId, Member member)
        {
            var hasVoted = _context.PollAnswers
                .Include(pa => pa.Votes)
                .Any(pa => pa.PollID == pollId && pa.Votes.Any(v => v.MemberID == member.MemberID));
            return hasVoted;
        }
    }
}
