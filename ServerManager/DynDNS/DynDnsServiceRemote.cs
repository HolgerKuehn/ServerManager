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

        //public override void UpdatePublicDnsIpAddress()
        //{
        //    base.UpdatePublicDnsIpAddress();

        //    this.UpdatePublicDnsIpAddress(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadIpAddressIDPublicIp, "set public IPs from " + this.Name + " (RemoteService) as updated");
        //}
    }
}
