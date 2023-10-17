
namespace blog.dachs.ServerManager
{
    public class DynDnsServiceRemote : DynDnsService
    {
        public DynDnsServiceRemote(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {

        }

        public override void GetIpAddress()
        {
            // get only DNS IP 
            base.GetPublicIpAddress();
        }
    }
}
