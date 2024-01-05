using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsFirewallRuleName : GlobalExtention
    {
        private string name;

        public DynDnsFirewallRuleName(Configuration configuration, string Name) : base(configuration)
        {
            name = Name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
