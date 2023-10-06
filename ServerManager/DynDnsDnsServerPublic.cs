namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServerPublic : DynDnsDnsServer
    {
        public DynDnsDnsServerPublic()
        {
            this.SetNetworkDnsServer(DynDnsIpAddressVersion.IPv4, new DynDnsIpAddress("45.90.28.58"));
            this.SetNetworkDnsServer(DynDnsIpAddressVersion.IPv6, new DynDnsIpAddress("2a07:a8c0::6d:cda2"));
        }
    }
}
