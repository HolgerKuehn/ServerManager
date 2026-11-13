namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsServiceRemote : DynDnsService
    {
        public DynDnsServiceRemote(Configuration configuration, Int32 dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
        }

        public override void GetIpAddress()
        {
            // get only DNS IP and Update IP in Domain
            this.UpdateLocalDomainAddress(base.GetPublicIpAddress());
        }
    }
}
