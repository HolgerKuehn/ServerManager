namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Collections;
    using System.Data;
    using blog.dachs.ServerManager;

    public class DynDnsFirewallRuleCollection : GlobalExtention, IList
    {
        private List<DynDnsFirewallRule> firewallRules;

        public DynDnsFirewallRuleCollection(Configuration configuration) : base(configuration)
        {
            firewallRules = new List<DynDnsFirewallRule>();
        }

        public List<DynDnsFirewallRule> FirewallRules
        {
            get { return firewallRules; }
            set { firewallRules = value; }
        }

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

        public void ReadFirewallRuleCollectionFromPowerShell()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, "read firewall rules from PowerShell"));

            string pshCommand = Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell_ReadFirewallRuleCollectionFromPowerShell);
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, pshCommand));

            // read powerShellOutput from PowerShell and add them to disc
            ProcessOutput powerShellOutput = PowerShell.ExecuteCommand(pshCommand);

            // add new firewall rules
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, "add firewall rule to disc"));

            string sqlCommandWriteFirewallRuleOriginal = Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell_WriteFirewallRuleCollection);
            string sqlCommandWriteFirewallReplace;
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, sqlCommandWriteFirewallRuleOriginal));

            int i = 4;
            while (true)
            {
                string firewallRuleName = PowerShell.CommandLine.GetProcessOutput(powerShellOutput, i);
                if (firewallRuleName == null)
                {
                    break;
                }
                else if (firewallRuleName.Trim() != string.Empty)
                {
                    sqlCommandWriteFirewallReplace = sqlCommandWriteFirewallRuleOriginal;
                    sqlCommandWriteFirewallReplace = sqlCommandWriteFirewallReplace.Replace("<DynDnsFirewallRuleName>", firewallRuleName.Trim());
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, sqlCommandWriteFirewallReplace));

                    Database.ExecuteCommand(sqlCommandWriteFirewallReplace);
                }

                i++;
            }

            PowerShell.CommandLine.DeleteProcessOutput(powerShellOutput);


            //// read firewall rules from disc
            //this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, "read firewall rules from disc"));

            //string sqlCommandReadFirewallRule = this.Database.GetCommand(Command.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell_ReadFirewallRuleCollectionFromDisc);
            //this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell, sqlCommandReadFirewallRule));

            //DataTable dataTable = this.Database.GetDataTable(sqlCommandReadFirewallRule);
            //DataRow dataRow = null;
            //int firewallRuleID = 0;
            //DynDnsFirewallRule firewallRule = null;

            //for (int row = 0; row < dataTable.Rows.Count; row++)
            //{
            //    dataRow = dataTable.Rows[row];
            //    firewallRuleID = Convert.ToInt32(dataRow[0].ToString());

            //    firewallRule = new DynDnsFirewallRule(this.Configuration, firewallRuleID);

            //    this.FirewallRules.Add(firewallRule);
            //}
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
    }
}
