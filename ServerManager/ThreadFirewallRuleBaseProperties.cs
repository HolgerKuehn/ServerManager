namespace blog.dachs.ServerManager
{
    using System.Data;

    public class ThreadFirewallRuleBaseProperties : ThreadWorker
    {
        private DynDnsServerLocal server;

        public ThreadFirewallRuleBaseProperties(Configuration configuration, DynDnsServerLocal dynDnsServerLocal) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "created new ThreadFirewallRuleBaseProperties"));
            this.server = dynDnsServerLocal;
        }

        public DynDnsServerLocal Server
        {
            get { return server; }
            set { this.server = value; }
        }

        public override void Work()
        {
            DynDnsFirewallRuleCollection firewallRuleCollection = new DynDnsFirewallRuleCollection(this.Configuration);

            while (true)
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ThreadDynDns_ThreadDynDns, "refreshing firewall rule base properties"));

                firewallRuleCollection.Clear();
                firewallRuleCollection.ReadFirewallRuleCollectionFromPowerShell();
                //server.FirewallRuleCollection = firewallRuleCollection;

                Thread.Sleep(21600000);
            }
        }
    }
}
