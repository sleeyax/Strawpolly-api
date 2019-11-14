using System.Net;
using System.Net.Mail;
using strawpoll.Config;

namespace strawpoll.Email.Clients
{
    public abstract class SmtpEmailClient : IEmail
    {
        protected readonly SmtpClient _smtpClient;

        protected SmtpEmailClient(EmailSetting emailSettings)
        {
            _smtpClient = new SmtpClient(emailSettings.Host, emailSettings.Port)
            {
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                EnableSsl = true
            };
        }

        public void Send(string fromAddress, string toAddress, string title, string body)
        {
            _smtpClient.Send(fromAddress, toAddress, title, body);
        }
    }
}