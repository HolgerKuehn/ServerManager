using System.Data;
using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
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

            FirewallRuleID = firewallRuleID;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, "read firewall rule properties"));
            sqlCommandReadFirewallRule = Database.GetCommand(Command.DynDnsFirewallRule_DynDnsFirewallRule);
            sqlCommandReadFirewallRule = sqlCommandReadFirewallRule.Replace("<DynDnsFirewallRuleID>", FirewallRuleID.ToString());
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, sqlCommandReadFirewallRule));

            DataTable dataTable = Database.GetDataTable(sqlCommandReadFirewallRule);
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
                    Name = firewallRuleName;

                if (firewallRuleDisplayName != null)
                    DisplayName = firewallRuleDisplayName;

                if (firewallRuleEnabled != null && firewallRuleEnabled == "0")
                {
                    Enabled = false;
                }
                else
                {
                    Enabled = true;
                }

                RemoteAddresses = new DynDnsIpAddressCollection(Configuration);
                RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
                RemoteAddresses.ReferenceId = FirewallRuleID;

                Services = new DynDnsServiceCollection(Configuration);
                Active = false;
            }
        }

        public int FirewallRuleID
        {
            get { return firewallRuleID; }
            set { firewallRuleID = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public DynDnsIpAddressCollection RemoteAddresses
        {
            get { return remoteAddresses; }
            set { remoteAddresses = value; }
        }

        public DynDnsServiceCollection Services
        {
            get { return services; }
            set { services = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public void ReadFirewallRuleBaseProperties()
        {
            string pshCommand;
            ProcessOutput processOutput;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, "read base properties from firewall rule \"" + Name + "\""));

            // read DisplayName
            pshCommand = Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_DisplayName);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name);
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = PowerShell.ExecuteCommand(pshCommand);
            string? firewallRuleDisplayName = CommandLine.GetProcessOutput(processOutput, 4);

            if (firewallRuleDisplayName != null)
                DisplayName = firewallRuleDisplayName;

            // read Enabled
            pshCommand = Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_Enabled);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name);
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = PowerShell.ExecuteCommand(pshCommand);
            string? firewallRuleEnabled = PowerShell.CommandLine.GetProcessOutput(processOutput, 4);

            if (firewallRuleEnabled != null && firewallRuleEnabled.Trim().ToLower() == "false")
            {
                Enabled = false;
            }
            else
            {
                Enabled = true;
            }

            PowerShell.CommandLine.DeleteProcessOutput(processOutput);
        }
    }
}