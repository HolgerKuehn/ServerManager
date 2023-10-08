namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServerPublic : DynDnsDnsServer
    {
        public DynDnsDnsServerPublic()
        {
            this.SetDynDnsDnsServer(DynDnsIpAddressVersion.IPv4, new DynDnsIpAddress("45.90.28.58"), 32);
            this.SetDynDnsDnsServer(DynDnsIpAddressVersion.IPv6, new DynDnsIpAddress("2a07:a8c0::6d:cda2"), 128);
        }
    }
}
