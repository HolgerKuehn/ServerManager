namespace blog.dachs.ServerManager
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
