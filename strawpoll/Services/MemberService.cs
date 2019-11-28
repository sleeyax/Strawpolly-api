using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using strawpoll.Config;
using strawpoll.Models;
using strawpoll.Security;

namespace strawpoll.Services
{
    public class MemberService : IMemberService
    {
        private readonly AppSettings _appSettings;
        private readonly DatabaseContext _databaseContext;

        public MemberService(IOptions<AppSettings> appSettingsOptions, DatabaseContext dbContext)
        {
            _appSettings = appSettingsOptions.Value;
            _databaseContext = dbContext;
        }

        public Member Authenticate(string email, string password)
        {
            var member = _databaseContext.Members.SingleOrDefault(x => x.Email == email && x.Salt != null && Hash.Validate(password, x.Salt, x.Password));

            if (member == null)  return null;

            member.Token = CreateJwt(member);
            member.Password = null;

            return member;
        }

        private string CreateJwt(Member member, int daysUntilExpiration = 7)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("MemberID", member.MemberID.ToString()),
                    new Claim("Email", member.Email),
                    new Claim("FirstName", member.FirstName),
                }),
                Expires = DateTime.UtcNow.AddDays(daysUntilExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}