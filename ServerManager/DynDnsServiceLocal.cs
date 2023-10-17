
namespace blog.dachs.ServerManager
{
    public class DynDnsServiceLocal : DynDnsService
    {
        public DynDnsServiceLocal(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            
        }

        public override void GetIpAddress()
        {
            // get public IP 
            base.GetPublicIpAddress();
        }
    }
}
