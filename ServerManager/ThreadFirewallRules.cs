namespace blog.dachs.ServerManager
{
    using blog.dachs.ServerManager.DynDNS;

    public class ThreadFirewallRules : ThreadWorker
    {
        public ThreadFirewallRules(Configuration configuration) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "created new ThreadFirewallRules"));
        }

        public override void Work()
        {
            DynDnsFirewallRuleCollection firewallRuleCollection = new DynDnsFirewallRuleCollection(this.Configuration);

            while (true)
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ThreadDynDns_ThreadDynDns, "refreshing firewall rule base properties"));

                // read current state
                firewallRuleCollection.Clear();
                firewallRuleCollection.ReadFirewallRuleCollectionFromPowerShell();

                // activate required rules
                firewallRuleCollection.Clear();
                firewallRuleCollection.EnableRequiredRules();

                // disable obsolete rules
                firewallRuleCollection.Clear();
                firewallRuleCollection.DisableObsoleteRules();

                Thread.Sleep(21600000);
            }
        }
    }
}
