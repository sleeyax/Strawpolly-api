using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using strawpoll.Config;
using strawpoll.Models;

namespace strawpoll.Services
{
    public class MemberService : IMemberService
    {
        private readonly AppSettings _appSettings;
        private readonly DatabaseContext _databaseContext;

        public MemberService(IOptions<AppSettings> appSettings, DatabaseContext dbContext)
        {
            _appSettings = appSettings.Value;
            _databaseContext = dbContext;
        }

        public Member Authenticate(string email, string password)
        {
            var member = _databaseContext.Members.SingleOrDefault(x => x.Email == email && x.Password == password);

            if (member == null)  return null;
           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("MemberID", member.MemberID.ToString()),
                    new Claim("Email", member.Email),
                    new Claim("FirstName", member.FirstName), 
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            member.Token = tokenHandler.WriteToken(token);

            member.Password = null;

            return member;
        }
    }
}