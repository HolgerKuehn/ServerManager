namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServerPublic : DynDnsDnsServer
    {
        public DynDnsDnsServerPublic(Configuration configuration) : base(configuration)
        {
            this.DynDnsDnsServerType = DynDnsDnsServerType.Public;

            this.SetDynDnsDnsServer(DynDnsIpAddressVersion.IPv4, new DynDnsIpAddress(this.Configuration, "45.90.28.58"), 32);
            this.SetDynDnsDnsServer(DynDnsIpAddressVersion.IPv6, new DynDnsIpAddress(this.Configuration, "2a07:a8c0::6d:cda2"), 128);
        }
    }
}
