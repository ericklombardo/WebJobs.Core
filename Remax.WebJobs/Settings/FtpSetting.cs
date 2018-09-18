using FluentFTP;

namespace Remax.WebJobs.Settings
{
    public class FtpSetting
    {
        public int SocketPollInterval { get; set; }
        public int ConnectTimeout { get; set; }
        public int ReadTimeout { get; set; }
        public int DataConnectionConnectTimeout { get; set; }
        public int DataConnectionReadTimeout { get; set; }
        public FtpDataConnectionType DataConnectionType { get; set; }

    }
}
