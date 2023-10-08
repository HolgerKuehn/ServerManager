using System;

namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServer
    {
        private Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress> dynDnsDnsServer;

        public DynDnsDnsServer()
        {
            this.dynDnsDnsServer = new Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress>();
        }

        public DynDnsIpAddress GetDynDnsDnsServer(DynDnsIpAddressVersion ipVersion)
        {
            return this.dynDnsDnsServer[ipVersion];
        }

        public void SetDynDnsDnsServer(DynDnsIpAddressVersion DynDnsIpAddressVersion, DynDnsIpAddress dynDnsDnsServerAddress)
        {
            this.dynDnsDnsServer[DynDnsIpAddressVersion] = dynDnsDnsServerAddress;
        }

        public void SetDynDnsDnsServer(DynDnsIpAddressVersion DynDnsIpAddressVersion, DynDnsIpAddress dynDnsDnsServerAddress, byte dynDnsDnsServerIpAddressPrefixLength)
        {
            this.dynDnsDnsServer[DynDnsIpAddressVersion] = dynDnsDnsServerAddress;
            this.SetDynDnsDnsServerIpAddressPrefixLength(DynDnsIpAddressVersion, dynDnsDnsServerIpAddressPrefixLength);
        }

        public void SetDynDnsDnsServerIpAddressPrefixLength(DynDnsIpAddressVersion DynDnsIpAddressVersion, byte dynDnsDnsServerIpAddressPrefixLength)
        {
            this.dynDnsDnsServer[DynDnsIpAddressVersion].PrefixLength = dynDnsDnsServerIpAddressPrefixLength;
        }
    }
}
