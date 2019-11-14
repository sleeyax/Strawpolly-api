using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using strawpoll.Api.Requests;
using strawpoll.Api.Responses;
using strawpoll.Config;
using strawpoll.Email;
using strawpoll.Email.Clients;
using strawpoll.Models;

namespace strawpoll.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendController : AuthController
    {
        private readonly IEmail _mailClient;
        private AppSettings _appSettings;

        public FriendController(IOptions<AppSettings> appSettings, DatabaseContext context) : base(context)
        {
            // TODO: check if the environment is production, then set it to SendGrid
            _mailClient = new MailTrap(appSettings);
            _appSettings = appSettings.Value;
        }

        // GET: api/friends
        // Returns a list of this Member friends (incl. all FriendStatuses)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendResponse>>> GetFriends()
        {
            Member member = GetAuthenticatedMember();
            return await _context.Friends
                .Where(f => (f.MemberID == member.MemberID || f.MemberFriendID == member.MemberID) && HasModPermissions(f, member))
                .Include(m => m.MemberFriend)
                .Include(m => m.Member)
                .Select(f => ToFriendResponse(f, member))
            .ToListAsync();
        }

        private bool HasModPermissions(Friend f, Member m)
        {
            // the user who modified the FriendStatus should always have the option to undo the change, or change it to something else again.
            // Example: member A blocks member B. Member B can't see member A in his friend list, but member A should still have the option to undo the block.

            // member A
            if (f.MemberWhoModifiedID == m.MemberID) return true;

            // member B
            if (f.MemberWhoModifiedID != m.MemberID)
            {
                switch (f.FriendStatus)
                {
                    case FriendStatus.Blocked:
                    case FriendStatus.Declined:
                    // NOTE: pending is added here because the api/friends/requests endpoint will be used to handle pending FRs
                    case FriendStatus.Pending:
                        return false;
                    // case FriendStatus.Accepted:
                    default:
                        return true;
                }
            }

            // no friends
            return false;
        }

        private FriendResponse ToFriendResponse(Friend f, Member m)
        {
            Member friend = f.MemberID == m.MemberID ? f.MemberFriend : f.Member;

            return new FriendResponse
            {
                Friend = new Member
                {
                    MemberID = friend.MemberID,
                    Email = friend.Email,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName
                },
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

        // GET: api/friends/accepted
        [HttpGet("accepted")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendResponse>>> GetAcceptedFriendRequests()
        {
            Member member = GetAuthenticatedMember();
            return await _context.Friends
                .Where(f => (f.MemberID == member.MemberID || f.MemberFriendID == member.MemberID) && f.FriendStatus == FriendStatus.Accepted)
                .Include(f => f.Member)
                .Include(f => f.MemberFriend)
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
            if (friend.MemberID != member.MemberID && friend.MemberFriendID != member.MemberID)
                return NotFound();

            friend.FriendStatus = request.FriendStatus;
            friend.MemberWhoModifiedID = member.MemberID;

            _context.Entry(friend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Friends.Any(e => e.FriendID == id))
                    return NotFound();
                
                throw;
            }

            return NoContent();
        }

        // POST: api/friends
        // sends a new friend request
        [HttpPost]
        public async Task<ActionResult<Friend>> PostFriend(FriendRequest request)
        {
            Member member = GetAuthenticatedMember();

            foreach (var email in request.FriendEmails)
            {
                // do not allow members to send themselves a FR
                if (email == member.Email) continue;

                Member friend = _context.Members.FirstOrDefault(m => m.Email == email);

                // user with that email doesn't exist yet, add it to db and send link to email address to create account
                if (friend == null)
                {
                    // create new account with this email
                    friend = new Member
                    {
                        Email = email,
                        CreationKey = Guid.NewGuid().ToString()
                    };
                    _context.Members.Add(friend);
                    await _context.SaveChangesAsync();

                    // send invitation email
                    _mailClient.Send(
                        "admin@strawpolly.com",
                        email,
                        "Strawpolly invitation link",
                        $"Hello,\n\nPlease click the link below to create a Strawpolly account:\n\n{_appSettings.FrontEndUrl}/register/{friend.CreationKey}"
                    );
                }

                // insert pending friend request into db if they are not in the friends table yet
                if (!AreFriends(member, friend))
                    _context.Friends.Add(new Friend
                    {
                        MemberID = member.MemberID,
                        MemberFriendID = friend.MemberID,
                        MemberWhoModifiedID = member.MemberID,
                        FriendStatus = FriendStatus.Pending
                    });
            }

            await _context.SaveChangesAsync();

            // return CreatedAtAction("GetFriend", new { id = friend.FriendID }, friend);
            return Ok();
        }

        // DELETE: api/friends/5
        // remove friend
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

        private bool AreFriends(Member member, Member friend)
        {
            return _context.Friends.Any(f => 
                f.MemberID == member.MemberID && f.MemberFriendID == friend.MemberID || 
                f.MemberID == friend.MemberID && f.MemberFriendID == member.MemberID);
        }
    }
}
