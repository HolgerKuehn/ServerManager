using System;

namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServer
    {
        private Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress> networkDnsServer;

        public DynDnsDnsServer()
        {
            this.networkDnsServer = new Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress>();
        }

        public DynDnsIpAddress GetNetworkDnsServer(DynDnsIpAddressVersion ipVersion)
        {
            return this.networkDnsServer[ipVersion];
        }

        public void SetNetworkDnsServer(DynDnsIpAddressVersion DynDnsIpAddressVersion, DynDnsIpAddress networkIPAddress)
        {
            this.networkDnsServer[DynDnsIpAddressVersion] = networkIPAddress;
        }
    }
}
