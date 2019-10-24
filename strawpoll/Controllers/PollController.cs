﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Api.Requests;
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
        public async Task<IEnumerable<Poll>> GetMemberPolls()
        {
            Member member = GetAuthenticatedMember();

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
            Member member = GetAuthenticatedMember();

            var poll = _context.Polls
                .Include(p => p.Answers)
                .FirstOrDefault(p => p.Creator == member && p.PollID == id);

            if (poll == null)
                return NotFound();
            

            return poll;
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
                .FirstOrDefault(p => p.PollID == request.PollID);

            if (poll == null) return NotFound();

            poll.Answers = request.Answers.Select(a => new PollAnswer() {Answer = a.Answer}).ToList();
            poll.Name = request.Name;

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
                Name = request.Name
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