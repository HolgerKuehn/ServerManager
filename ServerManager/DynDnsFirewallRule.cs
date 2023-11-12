using System.Data;

namespace blog.dachs.ServerManager
{
    public class DynDnsFirewallRule : GlobalExtention
    {
        private int firewallRuleID;
        private string name;
        private string displayName;
        private bool enabled;
        private DynDnsIpAddressCollection remoteAddresses;

        /* properties for settings */
        private DynDnsServiceCollection services;
        private bool active;

        public DynDnsFirewallRule(Configuration configuration, int firewallRuleID) : base(configuration)
        {
            string sqlCommandReadFirewallRule;
            
            this.FirewallRuleID = firewallRuleID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, "read firewall rule properties"));
            sqlCommandReadFirewallRule = this.HandlerDatabase.GetCommand(Command.DynDnsFirewallRule_DynDnsFirewallRule);
            sqlCommandReadFirewallRule = sqlCommandReadFirewallRule.Replace("<DynDnsFirewallRuleID>", this.FirewallRuleID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, sqlCommandReadFirewallRule));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommandReadFirewallRule);
            DataRow dataRow = null;
            string firewallRuleName = string.Empty;
            string firewallRuleDisplayName = string.Empty;
            string firewallRuleEnabled = string.Empty;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                firewallRuleName = dataRow[0].ToString();
                firewallRuleDisplayName = dataRow[1].ToString();
                firewallRuleEnabled = dataRow[2].ToString();

                if (firewallRuleName != null)
                    this.Name = firewallRuleName;

                if (firewallRuleDisplayName != null)
                    this.DisplayName = firewallRuleDisplayName;

                if (firewallRuleEnabled != null && firewallRuleEnabled == "0")
                {
                    this.Enabled = false;
                }
                else
                {
                    this.Enabled = true;
                }

                this.RemoteAddresses = new DynDnsIpAddressCollection(this.Configuration);
                this.RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
                this.RemoteAddresses.ReferenceId = this.FirewallRuleID;

                this.Services = new DynDnsServiceCollection(this.Configuration);
                this.Active = false;
            }
        }

        public int FirewallRuleID
        {
            get { return this.firewallRuleID; }
            set { this.firewallRuleID = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        public DynDnsIpAddressCollection RemoteAddresses
        {
            get { return this.remoteAddresses; }
            set { this.remoteAddresses = value; }
        }

        public DynDnsServiceCollection Services
        { 
            get { return this.services; }
            set { this.services = value; }
        }

        public bool Active
        {
            get { return this.active; }
            set { this.active = value; }
        }

        public void ReadFirewallRuleBaseProperties()
        {
            string pshCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, "read base properties from firewall rule \"" + this.Name + "\""));

            // read DisplayName
            pshCommand = this.HandlerDatabase.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_DisplayName);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", this.Name);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            List<string> firewallRuleDisplayName = this.HandlerPowerShell.Command(pshCommand);

            // remove caption
            firewallRuleDisplayName.RemoveRange(0, 3);

            // add DisplayName to firewallRule
            if (firewallRuleDisplayName[0] != null)
                this.DisplayName = firewallRuleDisplayName[0];

            // read Enabled
            pshCommand = this.HandlerDatabase.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_Enabled);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", this.Name);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            List<string> firewallRuleEnabled = this.HandlerPowerShell.Command(pshCommand);

            // remove caption
            firewallRuleEnabled.RemoveRange(0, 3);

            // add Enabled to firewallRule
            if (firewallRuleEnabled[0] != null && firewallRuleEnabled[0].Trim().ToLower() == "false")
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }
        }
    }
}