using System.Net.Mail;

namespace Remax.WebJobs.Settings
{
    public class EmailSetting
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public string PickupDirectoryLocation { get; set; }
    }
}
