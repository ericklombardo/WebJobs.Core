namespace Remax.WebJobs.Settings
{
    public class SiteSetting
    {
        public string FtpServer { get; set; }
        public string[] RootFolder { get; set; }
        public string DestinationFolder { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
