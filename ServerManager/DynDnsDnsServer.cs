namespace blog.dachs.ServerManager
{
    using System.Net.Sockets;

    public enum DnsServerType
    {
        Private = 1,
        Public = 2
    }

    public abstract class DynDnsDnsServer : GlobalExtention
    {
        private DnsServerType dynDnsDnsServerType;
        private Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress> dynDnsDnsServer;

        public DynDnsDnsServer(Configuration configuration) : base(configuration)
        {
            this.dynDnsDnsServer = new Dictionary<DynDnsIpAddressVersion, DynDnsIpAddress>();
        }

        public DnsServerType DynDnsDnsServerType
        {
            get { return this.dynDnsDnsServerType; }
            set { this.dynDnsDnsServerType = value; }
        }

        public DynDnsIpAddress GetDynDnsDnsServerIp()
        {
            if (Socket.OSSupportsIPv6)
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
