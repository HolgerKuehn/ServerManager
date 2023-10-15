
namespace blog.dachs.ServerManager
{
   using System.Net;
   using System.Net.NetworkInformation;
   using System.Net.Sockets;

    public class DynDnsServiceLocal : DynDnsService
    {
        public DynDnsServiceLocal(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            
        }
    }
}
