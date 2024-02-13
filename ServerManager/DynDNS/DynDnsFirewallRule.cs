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
        private bool active;

        public DynDnsFirewallRule(Configuration configuration) : base(configuration)
        {
            this.FirewallRuleID = 0;
            this.Name = string.Empty;
            this.DisplayName = string.Empty;
            this.Enabled = false;
            this.RemoteAddresses = new DynDnsIpAddressCollection(Configuration);
            this.RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
            this.RemoteAddresses.ReferenceId = 0;
            this.Active = false;
        }

        public DynDnsFirewallRule(Configuration configuration, int firewallRuleID) : base(configuration)
        {
            DataRow dataRow;
            string sqlCommandReadFirewallRule;
            string firewallRuleName;
            string firewallRuleDisplayName;
            string firewallRuleEnabled;
            string firewallRuleActive;

            this.FirewallRuleID = firewallRuleID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, "read firewall rule properties"));
            sqlCommandReadFirewallRule = this.Database.GetCommand(Command.DynDnsFirewallRule_DynDnsFirewallRule);
            sqlCommandReadFirewallRule = sqlCommandReadFirewallRule.Replace("<DynDnsFirewallRuleID>", this.FirewallRuleID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, sqlCommandReadFirewallRule));

            dataRow = this.Database.GetDataRow(sqlCommandReadFirewallRule, 0);
            firewallRuleName = string.Empty;
            firewallRuleDisplayName = string.Empty;
            firewallRuleEnabled = string.Empty;
            firewallRuleActive = string.Empty;

            if (dataRow != null)
            {
                firewallRuleName = dataRow[0].ToString();
                firewallRuleDisplayName = dataRow[1].ToString();
                firewallRuleEnabled = dataRow[2].ToString();
                firewallRuleActive = dataRow[2].ToString();

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

                if (firewallRuleActive != null && firewallRuleActive == "0")
                {
                    this.Active = false;
                }
                else
                {
                    this.Active = true;
                }

                this.Active = false;

                this.RemoteAddresses = new DynDnsIpAddressCollection(Configuration);
                this.RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
                this.RemoteAddresses.ReferenceId = this.FirewallRuleID;
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

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public void ReadFirewallRuleBaseProperties()
        {
            string pshCommand;
            ProcessOutput processOutput;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, "read base properties from firewall rule \"" + Name + "\""));

            // read DisplayName
            pshCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_DisplayName);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name.Replace("...", "*"));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = this.PowerShell.ExecuteCommand(pshCommand);
            string? firewallRuleDisplayName = this.CommandLine.GetProcessOutput(processOutput, 3);

            if (firewallRuleDisplayName != null)
                this.DisplayName = firewallRuleDisplayName;

            // read Enabled
            pshCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_Enabled);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name.Replace("...", "*"));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = this.PowerShell.ExecuteCommand(pshCommand);
            string? firewallRuleEnabled = this.PowerShell.CommandLine.GetProcessOutput(processOutput, 3);

            if (firewallRuleEnabled != null && firewallRuleEnabled.Trim().ToLower() == "false")
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }

            this.PowerShell.CommandLine.DeleteProcessOutput(processOutput);
        }

        public void PrepareFirewallRuleToDisc()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_PrepareFirewallRuleToDisc, "prepare Database for FirewallRule"));

            string sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_PrepareFirewallRuleToDisc);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_PrepareFirewallRuleToDisc, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleName>", this.Name);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_PrepareIpAddressToDisc, sqlCommand));

            this.Database.ExecuteCommand(sqlCommand);
        }

        public void WriteFirewallRuleToDisc()
        {
            this.PrepareFirewallRuleToDisc();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_WriteFirewallRuleToDisc, "write FirewallRule " + this.Name + " to disc"));

            string sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_WriteFirewallRuleToDisc);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_WriteFirewallRuleToDisc, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleName>", this.Name);
            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleDisplayName>", this.DisplayName);
            
            if (this.Enabled)
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleEnabled>", "1");
            }
            else
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleEnabled>", "0");
            }
            
            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleTimestamp>", ((DateTimeOffset)DateTime.UtcNow.ToLocalTime()).ToUnixTimeSeconds().ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_WriteFirewallRuleToDisc, sqlCommand));

            this.Database.ExecuteCommand(sqlCommand);
        }

        public void EnableFirewallRule()
        {
            this.PowerShell.ExecuteCommand("Get-NetFirewallRule -Name \'" + this.Name.Replace("...", "*") + "\' | Enable-NetFirewallRule");
        }

        public void DisableFirewallRule()
        {
            this.PowerShell.ExecuteCommand("Get-NetFirewallRule -Name \'" + this.Name.Replace("...", "*") + "\' | Disable-NetFirewallRule");
        }
    }
}