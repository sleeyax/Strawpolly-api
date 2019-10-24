using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    [Route("api/friends")]
    [ApiController]
    public class FriendController : AuthController
    {

        /* TODO: keep track of the user who last updated the FriendStatus.
           For example, how do we know which user blocked another user?
           We have to know this because only one user can 'lift' the block.
           -> Use the MemberWhoModified property (from model Friend) we added last time!
        */

        public FriendController(DatabaseContext context) : base(context) { }

        // GET: api/friends
        // Returns a list of this Member friends
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendResponse>>> GetFriends()
        {
            Member member = GetAuthenticatedMember();
            return await _context.Friends
                .Where(f => f.MemberID == member.MemberID || (f.MemberFriendID == member.MemberID && f.FriendStatus != FriendStatus.Pending))
                .Include(m => m.MemberFriend)
                .Include(m => m.Member)
                .Select(f => ToFriendResponse(f, member))
            .ToListAsync();
        }

        private FriendResponse ToFriendResponse(Friend f, Member m)
        {
            Member member = new Member();
            if (f.MemberID == m.MemberID)
            {
                member.Email = f.MemberFriend.Email;
                member.FirstName = f.MemberFriend.FirstName;
                member.LastName = f.MemberFriend.LastName;
            }
            else if (f.MemberFriendID == m.MemberID)
            {
                member.Email = f.Member.Email;
                member.FirstName = f.Member.FirstName;
                member.LastName = f.Member.LastName;
            }
            else
            {
                throw new Exception("This should not happen");
            }

            return new FriendResponse
            {
                Friend = member,
                FriendID = f.FriendID,
                FriendStatus = f.FriendStatus
            };
        }

        // GET: api/friends/requests
        // Returns a list of members that have sent the current user a friend request
        [HttpGet("requests")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendResponse>>> GetFriendRequests()
        {
            Member member = GetAuthenticatedMember();
            return await _context.Friends
                .Where(f => f.MemberFriendID == member.MemberID && f.FriendStatus == FriendStatus.Pending)
                .Include(f => f.Member)
                .Select(f => ToFriendResponse(f, member))
                .ToListAsync();
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
        // update friend status (accept, decline, ...) for incoming FR
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriend(long id, FriendStatusUpdateRequest request)
        {
            if (id != request.FriendID)
                return BadRequest();

            Member member = GetAuthenticatedMember();

            Friend friend = await _context.Friends.FindAsync(id);
            if (friend.MemberFriendID != member.MemberID)
                return NotFound(friend.MemberFriendID + " " + member.MemberID);

            friend.FriendStatus = request.FriendStatus;

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
            Friend friend = await _context.Friends.FindAsync(id);
            if (friend == null)
                return NotFound();

            Member member = GetAuthenticatedMember();
            if (friend.MemberID != member.MemberID && friend.MemberFriendID != member.MemberID)
                return BadRequest();

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
