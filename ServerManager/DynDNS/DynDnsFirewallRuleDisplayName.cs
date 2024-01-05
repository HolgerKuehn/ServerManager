using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsFirewallRuleDisplayName : GlobalExtention
    {
        private string displayName;

        public DynDnsFirewallRuleDisplayName(Configuration configuration, string displayName) : base(configuration)
        {
            this.displayName = displayName;
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
    }
}
