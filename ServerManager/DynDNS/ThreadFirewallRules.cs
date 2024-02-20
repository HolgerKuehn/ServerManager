namespace blog.dachs.ServerManager.DynDNS
{

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
                // read current state
                firewallRuleCollection.Clear();
                firewallRuleCollection.ReadFirewallRuleBaseProperties();

                // activate required rules
                firewallRuleCollection.Clear();
                firewallRuleCollection.EnableRequiredRules();

                // disable obsolete rules
                firewallRuleCollection.Clear();
                firewallRuleCollection.DisableObsoleteRules();

                // read and write RemoteAdresses
                firewallRuleCollection.Clear();
                firewallRuleCollection.ReadServiceFirewallRules();

                Thread.Sleep(120000);
            }
        }
    }
}
