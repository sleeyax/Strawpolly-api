namespace strawpoll.Config
{
    public class EmailSettings
    {
        public string Enabled { get; set; }
        public EmailSetting MailTrap { get; set; }
        public EmailSetting SendGrid { get; set; }
    }

    public class EmailSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}