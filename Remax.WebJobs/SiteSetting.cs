using System;
using System.Collections.Generic;
using System.Text;

namespace Remax.WebJobs
{
    public class SiteSetting
    {
        public string FtpServer { get; set; }
        public string RootFolder { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
