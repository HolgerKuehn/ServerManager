namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Collections;
    using System.Data;
    using blog.dachs.ServerManager;

    public class DynDnsFirewallRuleCollection : GlobalExtention, IList
    {
        private List<DynDnsFirewallRule> firewallRules;
        private DateTime lastSeenDate;

        public DynDnsFirewallRuleCollection(Configuration configuration) : base(configuration)
        {
            firewallRules = new List<DynDnsFirewallRule>();
        }

        public List<DynDnsFirewallRule> FirewallRules
        {
            get { return firewallRules; }
            set { firewallRules = value; }
        }

        public DateTime LastSeenDate
        {
            get { return lastSeenDate; }
            set { lastSeenDate = value; }
        }

        public void ReadFirewallRuleBaseProperties()
        {
            string sqlCommand;
            DataRow dataRow;
            long latestLastSeenTimestamp = 0;

            // get latest LastSeenDate
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadFirewallRules_Work, "read latest LastSeenDate"));
            sqlCommand = this.Database.GetCommand(Command.ThreadFirewallRules_Work_LatestLastSeenDate);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadFirewallRules_Work, sqlCommand));

            dataRow = this.Database.GetDataRow(sqlCommand, 0);

            if (dataRow != null)
            {
                latestLastSeenTimestamp = Convert.ToInt64(dataRow[0].ToString());
                this.LastSeenDate = DateTimeOffset.FromUnixTimeSeconds(latestLastSeenTimestamp).UtcDateTime.ToLocalTime();
            }

            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - latestLastSeenTimestamp > 6 /* h */ * 60 /* min */ * 60 /* sec */)
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ThreadFirewallRules_Work, "refreshing firewall rule base properties"));

                ProcessOutput powerShellOutput;
                string firewallRuleName;
                DynDnsFirewallRule firewallRule;
                DateTime lastSeenDate;

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, "read firewall rules from PowerShell"));

                string pshCommand = Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell_ReadFirewallRuleCollectionFromPowerShell);
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, pshCommand));

                // read powerShellOutput from PowerShell and add them to disc
                powerShellOutput = this.PowerShell.ExecuteCommand(pshCommand);
                lastSeenDate = DateTime.Now;
                this.LastSeenDate = lastSeenDate;

                // add new firewall rules
                int i = 3;
                while (true)
                {
                    firewallRuleName = this.PowerShell.CommandLine.GetProcessOutput(powerShellOutput, i);

                    if (firewallRuleName == null)
                    {
                        break;
                    }
                    else if (firewallRuleName.Trim() != string.Empty)
                    {
                        firewallRuleName = firewallRuleName.Trim();
                        firewallRule = new DynDnsFirewallRule(this.Configuration);
                        firewallRule.Name = firewallRuleName;
                        firewallRule.LastSeenDate = lastSeenDate;

                        firewallRule.ReadFirewallRuleBaseProperties();
                        firewallRule.WriteFirewallRuleToDisc();
                    }

                    i++;
                }

                this.PowerShell.CommandLine.DeleteProcessOutput(powerShellOutput);
            }
        }

        public void ReadActiveFirewallRules(bool enabled, bool active)
        {
            DataRow dataRow;
            string sqlCommand;
            int firewallRuleID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc, "read firewall rule collection"));
            sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollection_ActiveRules);
            
            if (enabled)
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleEnabled>", "1");
            }
            else
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleEnabled>", "0");
            }

            if (active)
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleActive>", "1");
            }
            else
            {
                sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleActive>", "0");
            }
            
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc, sqlCommand));


            int i = 0;
            while (true)
            {
                dataRow = this.Database.GetDataRow(sqlCommand, i);

                if (dataRow != null)
                {
                    firewallRuleID = Convert.ToInt32(dataRow[0].ToString());

                    this.Add(new DynDnsFirewallRule(this.Configuration, firewallRuleID));
                }
                else
                {
                    break;
                }

                i++;
            }
        }

        public void ReadServiceFirewallRules()
        {
            DataRow dataRow;
            string sqlCommand;
            int firewallRuleID;

            this.Clear();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc, "read firewall rules connected to services"));
            sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollection_ServiceRules);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc, sqlCommand));
            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleLastSeenTimestamp>", ((DateTimeOffset)this.LastSeenDate.ToUniversalTime()).ToUnixTimeSeconds().ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc, sqlCommand));

            int i = 0;
            while (true)
            {
                dataRow = this.Database.GetDataRow(sqlCommand, i);

                if (dataRow != null)
                {
                    firewallRuleID = Convert.ToInt32(dataRow[0].ToString());

                    this.Add(new DynDnsFirewallRule(this.Configuration, firewallRuleID));
                }
                else
                {
                    break;
                }

                i++;
            }

            foreach (DynDnsFirewallRule rule in this.FirewallRules)
            {
                rule.WriteRemoteAddress();
                rule.WriteFirewallRuleToDisc();
            }
        }

        public void EnableRequiredRules()
        {
            this.Clear();
            this.ReadActiveFirewallRules(false, true);

            foreach (DynDnsFirewallRule rule in this.FirewallRules)
            {
                rule.EnableFirewallRule();
                rule.Enabled = true;
                rule.WriteFirewallRuleToDisc();
            }
        }

        public void DisableObsoleteRules()
        {
            this.Clear();
            this.ReadActiveFirewallRules(true, false);

            foreach (DynDnsFirewallRule rule in this.FirewallRules)
            {
                rule.DisableFirewallRule();
                rule.Enabled = false;
                rule.WriteFirewallRuleToDisc();
            }
        }

        #region implement IList

        public object? this[int index] { get => ((IList)FirewallRules)[index]; set => ((IList)FirewallRules)[index] = value; }
        public bool IsFixedSize => ((IList)FirewallRules).IsFixedSize;

        public bool IsReadOnly => ((IList)FirewallRules).IsReadOnly;

        public int Count => ((ICollection)FirewallRules).Count;

        public bool IsSynchronized => ((ICollection)FirewallRules).IsSynchronized;

        public object SyncRoot => ((ICollection)FirewallRules).SyncRoot;

        public int Add(object? value)
        {
            return ((IList)FirewallRules).Add(value);
        }

        public void Clear()
        {
            ((IList)FirewallRules).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)FirewallRules).Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)FirewallRules).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)FirewallRules).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)FirewallRules).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)FirewallRules).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)FirewallRules).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)FirewallRules).RemoveAt(index);
        }

        #endregion

        public void Add(DynDnsFirewallRule DynDnsFirewallRule)
        {
            FirewallRules.Add(DynDnsFirewallRule);
        }

        public void AddRange(DynDnsFirewallRuleCollection DynDnsFirewallRuleCollection)
        {
            foreach (DynDnsFirewallRule DynDnsFirewallRule in DynDnsFirewallRuleCollection)
            {
                Add(DynDnsFirewallRule);
            }
        }
    }
}
