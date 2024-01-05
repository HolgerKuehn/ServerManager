using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsServerRemote : DynDnsServer
    {
        public DynDnsServerRemote(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
        }

        public override void GetIpAddress()
        {
            // get only public IP 
            base.GetPublicIpAddress();

            // invoke depending objects
            base.GetIpAddress();
        }
    }
}
