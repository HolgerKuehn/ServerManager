
namespace blog.dachs.ServerManager.DynDNS
{
    using blog.dachs.ServerManager;

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

        public override void UpdatePublicDnsIpAddress()
        {
        }
    }
}
