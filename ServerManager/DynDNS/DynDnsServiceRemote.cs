
using System.Data;
using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
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

        public override void UpdatePublicDnsIpAddress()
        {
            base.UpdatePublicDnsIpAddress();

            UpdatePublicDnsIpAddress(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadIpAddressIDPublicIp, "set public IPs from " + Name + " (RemoteService) as updated");
        }

    }
}
