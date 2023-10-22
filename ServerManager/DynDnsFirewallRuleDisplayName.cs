namespace blog.dachs.ServerManager
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
            get { return this.displayName; }
            set { this.displayName = value; }
        }
    }
}
