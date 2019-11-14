using Microsoft.Extensions.Options;
using strawpoll.Config;

namespace strawpoll.Email.Clients
{
    public class SendGrid : SmtpEmailClient
    {
        public SendGrid(IOptions<AppSettings> appSettings) : base(appSettings.Value.EmailSettings.SendGrid) { }
    }
}