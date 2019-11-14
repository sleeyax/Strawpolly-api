using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using strawpoll.Api.Requests;
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

        // GET: api/members
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members.ToListAsync();
        }

        // GET: api/members/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(long id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/members/key/abd4-889-xxx
        // get member by CreationKey
        [HttpGet("key/{creationKey}")]
        public async Task<ActionResult<Member>> GetMemberByCreationKey(string creationKey)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.CreationKey == creationKey);

            if (member == null)
                return NotFound();

            return member;
        }

        // PUT: api/members/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(long id, Member member)
        {
            if (id != member.MemberID)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(RegisterRequest request)
        {
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

            return CreatedAtAction("GetMember", new { id = member.MemberID }, member);
        }

        // POST: api/members/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult<Member>> AuthenticateMember([FromBody] LoginRequest request)
        {
            var authenticatedMember = _memberService.Authenticate(request.Email, request.Password);

            if (authenticatedMember == null)
                return BadRequest(new {message = "Invalid username or password!"});

            return Ok(authenticatedMember);
        }

        // DELETE: api/members/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Member>> DeleteMember(long id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return member;
        }

        private bool MemberExists(long id)
        {
            return _context.Members.Any(e => e.MemberID == id);
        }
    }
}
