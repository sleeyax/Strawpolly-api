using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Api.Requests;
using strawpoll.Api.Responses;
using strawpoll.Models;
using strawpoll.Services;

namespace strawpoll.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IMemberService _memberService;

        public MemberController(DatabaseContext context, IMemberService memberService)
        {
            _context = context;
            _memberService = memberService;
        }

        /// <summary>
        /// get member email address by creation key
        /// </summary>
        /// <param name="creationKey">unique key (GUID)</param>
        /// <returns></returns>
        // GET: api/members/key/abd4-889-xxx
        [HttpGet("key/{creationKey}")]
        public async Task<ActionResult<string>> GetMemberByCreationKey(string creationKey)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.CreationKey == creationKey);

            if (member == null)
                return NotFound("Invalid key!");

            return member.Email;
        }

        /// <summary>
        /// register account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<MemberResponse>> PostMember(RegisterRequest request)
        {
            // check if member with that email address already exists
            if (_context.Members.FirstOrDefault(m => m.Email == request.Email) != null)
                return BadRequest("You already have an account using that email address!");

            Member member;

            // update using CreationKey
            if (request.CreationKey != null)
            {
                member = _context.Members.FirstOrDefault(m => m.CreationKey == request.CreationKey);
                if (member == null)
                    return NotFound("Invalid creation key!");
                member.CreationKey = null;
            }
            else
            {
                // add new member
                member = new Member();
            }
            
            member.Email = request.Email;
            member.FirstName = request.FirstName;
            member.LastName = request.LastName;
            member.Password = request.Password;

            if (request.CreationKey != null)
                _context.Entry(member).State = EntityState.Modified;
            else
                _context.Members.Add(member);
            
            await _context.SaveChangesAsync();

            return Ok(ToMemberResponse(member));
        }

        /// <summary>
        /// Converts an instance of Member to MemberResponse
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private MemberResponse ToMemberResponse(Member member)
        {
            return new MemberResponse
            {
                MemberID = member.MemberID,
                Email = member.Email,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Token = member.Token
            };
        }

        /// <summary>
        /// log in
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/members/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult<Member>> AuthenticateMember([FromBody] LoginRequest request)
        {
            var authenticatedMember = _memberService.Authenticate(request.Email, request.Password);

            if (authenticatedMember == null)
                return BadRequest(new {message = "Invalid username or password!"});

            return Ok(ToMemberResponse(authenticatedMember));
        }
    }
}
