using System;

namespace blog.dachs.ServerManager
{
    public enum DynDnsDnsServerType
    {
        Private = 1,
        Public = 2
    }

    public abstract class DynDnsDnsServer : GlobalExtention
    {
        private DynDnsDnsServerType dynDnsDnsServerType;
        private Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress> dynDnsDnsServer;

        public DynDnsDnsServer(Configuration configuration) : base(configuration)
        {
            this.dynDnsDnsServer = new Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress>();
        }

        public DynDnsDnsServerType DynDnsDnsServerType
        {
            get { return this.dynDnsDnsServerType; }
            set { this.dynDnsDnsServerType = value; }
        }

        public DynDnsIpAddress GetDynDnsDnsServerIp()
        {
            if (System.Net.Sockets.Socket.OSSupportsIPv6)
            {
                return this.GetDynDnsDnsServerIp(DynDnsIpAddressVersion.IPv6);
            }
            else
            {
                return this.GetDynDnsDnsServerIp(DynDnsIpAddressVersion.IPv4);
            }
        }

        public DynDnsIpAddress GetDynDnsDnsServerIp(DynDnsIpAddressVersion ipVersion)
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
