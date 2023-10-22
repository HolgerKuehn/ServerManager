namespace blog.dachs.ServerManager
{
    public class DynDnsFirewallRuleName : GlobalExtention
    {
        private string name;

        public DynDnsFirewallRuleName(Configuration configuration, string Name) : base(configuration)
        {
            this.name = Name;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
