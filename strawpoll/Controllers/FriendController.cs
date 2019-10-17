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
    [Route("api/friends")]
    [ApiController]
    public class FriendController : AuthController
    {
        public FriendController(DatabaseContext context) : base(context) { }

        // GET: api/friends
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            return await _context.Friends.ToListAsync();
        }

        // GET: api/friends/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Friend>> GetFriend(long id)
        {
            var friend = await _context.Friends.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend;
        }

        // PUT: api/friends/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriend(long id, Friend friend)
        {
            if (id != friend.FriendID)
            {
                return BadRequest();
            }

            _context.Entry(friend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendExists(id))
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

        // POST: api/friends
        [HttpPost]
        public async Task<ActionResult<Friend>> PostFriend(FriendRequest request)
        {
            Member member = GetAuthenticatedMember();

            if (request.MemberID != member.MemberID)
                return NotFound();

            foreach (var email in request.FriendEmails)
            {
                Member friend = _context.Members.FirstOrDefault(m => m.Email == email);
                if (friend == null)
                {
                    // TODO: send invitation email
                    break;
                }
                // friend exists, insert pending friend request into db
                _context.Friends.Add(new Friend
                {
                    MemberID = member.MemberID,
                    MemberFriendID = friend.MemberID,
                    FriendStatus = FriendStatus.Pending
                });
            }

            await _context.SaveChangesAsync();

            // return CreatedAtAction("GetFriend", new { id = friend.FriendID }, friend);
            return Ok();
        }

        // DELETE: api/friends/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Friend>> DeleteFriend(long id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }

            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return friend;
        }

        private bool FriendExists(long id)
        {
            return _context.Friends.Any(e => e.FriendID == id);
        }
    }
}
