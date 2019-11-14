using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using strawpoll.Config;

namespace strawpoll.Email.Clients
{
    public class MailTrap : SmtpEmailClient
    {
        public MailTrap(IOptions<AppSettings> appSettings) : base(appSettings.Value.EmailSettings.MailTrap) { }
    }
}