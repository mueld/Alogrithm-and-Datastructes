using System;
using System.Net;
using TwinCAT.Ads;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection
{
    public class TC3ConnectionProps
    {
        #region props

        public Version Version { get; set; } = new Version(0, 0, 0);
        public IPAddress IPAddress { get; set; } = IPAddress.Parse("127.0.0.1");
        public AmsAddress AmsAddress { get; set; } = new AmsAddress(851);

        #endregion
        #region methods

        public void SetIPAddress(string ipAddress)
        {
            IPAddress = IPAddress.Parse(ipAddress);
        }

        public void SetAmsAddress(string amsAddress, int portNumber)
        {
            AmsAddress = new AmsAddress(amsAddress, portNumber);
        }

        #endregion
    }
}
