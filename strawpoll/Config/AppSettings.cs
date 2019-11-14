namespace strawpoll.Config
{
    public class AppSettings
    {
        public string JwtSecret { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public string FrontEndUrl { get; set; }
    }
}