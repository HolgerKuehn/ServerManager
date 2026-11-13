using System.Data;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsFirewallRule : GlobalExtention
    {
        private Int32 firewallRuleID;
        private String name;
        private String displayName;
        private bool enabled;
        private DateTime lastSeenDate;
        private DateTime lastWriteDate;
        private DynDnsIpAddressCollection remoteAddresses;

        /* properties for settings */
        private bool active;

        public DynDnsFirewallRule(Configuration configuration) : base(configuration)
        {
            this.FirewallRuleID = 0;
            this.Name = String.Empty;
            this.DisplayName = String.Empty;
            this.Enabled = false;
            this.RemoteAddresses = new DynDnsIpAddressCollection(Configuration);
            this.RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
            this.RemoteAddresses.ReferenceId = 0;
            this.Active = false;
        }

        public DynDnsFirewallRule(Configuration configuration, Int32 firewallRuleID) : base(configuration)
        {
            DataRow dataRow;
            String sqlCommandReadFirewallRule;
            String firewallRuleName;
            String firewallRuleDisplayName;
            String firewallRuleEnabled;
            String firewallRuleActive;
            long lastSeenTimestamp;
            long lastWriteTimestamp;

            this.FirewallRuleID = firewallRuleID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, "read firewall rule properties"));
            sqlCommandReadFirewallRule = this.Database.GetCommand(Command.DynDnsFirewallRule_DynDnsFirewallRule);
            sqlCommandReadFirewallRule = sqlCommandReadFirewallRule.Replace("<DynDnsFirewallRuleID>", this.FirewallRuleID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_DynDnsFirewallRule, sqlCommandReadFirewallRule));

            dataRow = this.Database.GetDataRow(sqlCommandReadFirewallRule, 0);
            firewallRuleName = String.Empty;
            firewallRuleDisplayName = String.Empty;
            firewallRuleEnabled = String.Empty;
            firewallRuleActive = String.Empty;

            if (dataRow != null)
            {
                firewallRuleName = dataRow[0].ToString();
                firewallRuleDisplayName = dataRow[1].ToString();
                firewallRuleEnabled = dataRow[2].ToString();
                firewallRuleActive = dataRow[3].ToString();
                lastSeenTimestamp = Convert.ToInt64(dataRow[4].ToString()); ;
                lastWriteTimestamp = Convert.ToInt64(dataRow[5].ToString()); ;

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
                
                this.LastSeenDate = DateTimeOffset.FromUnixTimeSeconds(lastSeenTimestamp).UtcDateTime.ToLocalTime();
                this.LastWriteDate = DateTimeOffset.FromUnixTimeSeconds(lastWriteTimestamp).UtcDateTime.ToLocalTime();

                this.RemoteAddresses = new DynDnsIpAddressCollection(Configuration);
                this.RemoteAddresses.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
                this.RemoteAddresses.ReferenceId = this.FirewallRuleID;
            }
        }

        public Int32 FirewallRuleID
        {
            get { return firewallRuleID; }
            set { firewallRuleID = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public String DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public DateTime LastWriteDate
        {
            get { return this.lastWriteDate; }
            set
            {
                if (!this.LastWriteDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.lastWriteDate = value;
            }
        }

        public DateTime LastSeenDate
        {
            get { return this.lastSeenDate; }
            set
            {
                if (!this.LastSeenDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.lastSeenDate = value;
            }
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
            String pshCommand;
            ProcessOutput processOutput;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, "read base properties from firewall rule \"" + Name + "\""));

            // read DisplayName
            pshCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_DisplayName);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name.Replace("...", "*"));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = this.PowerShell.ExecuteCommand(pshCommand);
            String columnNameDisplayName = this.CommandLine.GetProcessOutput(processOutput, 1);
            String firewallRuleDisplayName = this.CommandLine.GetProcessOutput(processOutput, 3);

            if (
                columnNameDisplayName != null && columnNameDisplayName.Trim() == "DisplayName" && 
                firewallRuleDisplayName != null
            )
            {
                this.DisplayName = firewallRuleDisplayName;
            }

            this.PowerShell.CommandLine.DeleteProcessOutput(processOutput);

            // read Enabled
            pshCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_ReadFirewallRuleBaseProperties_Enabled);
            pshCommand = pshCommand.Replace("<DynDnsFirewallRuleName>", Name.Replace("...", "*"));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_ReadFirewallRuleBaseProperties, pshCommand));
            processOutput = this.PowerShell.ExecuteCommand(pshCommand);
            
            String columnNameEnabled = this.PowerShell.CommandLine.GetProcessOutput(processOutput, 1);
            String firewallRuleEnabled = this.PowerShell.CommandLine.GetProcessOutput(processOutput, 3);

            if (columnNameEnabled != null && columnNameEnabled.Trim() == "Enabled") { 
            
                if (firewallRuleEnabled != null && firewallRuleEnabled.Trim().ToLower() == "false")
                {
                    this.Enabled = false;
                }
                else
                {
                    this.Enabled = true;
                }

            }

            this.PowerShell.CommandLine.DeleteProcessOutput(processOutput);
        }

        public void ReadRemoteAddress()
        {
            //powerShellResult = PowerShellHandler.Command("Get-NetFirewallRule -Name \'" + firewallRule.Name + "\' | Get-NetFirewallAddressFilter | Select-Object RemoteAddress");
            //List<String> powerShellResultList = powerShellResult.Split("\r\n").ToList();
            //if (powerShellResultList.Count < 3) return; 
            //powerShellResultList.RemoveRange(0, 3);
            //
            //// LocalSubnet, DNS, DHCP, WINS, DefaultGateway, Internet, Intranet, IntranetRemoteAccess, PlayToDevice
            //if (powerShellResultList.Count > 0)
            //{
            //    if (powerShellResultList.ElementAt(0).Trim().ToLower() == "any")
            //    {
            //        firewallRule.RemoteAddresses = new IpCollection();
            //    }
            //    else if (powerShellResultList.ElementAt(0).Trim().ToLower() == "localsubnet")
            //    {
            //        firewallRule.RemoteAddresses = new IpCollection();
            //    }
            //    else if (powerShellResultList.ElementAt(0).Trim().ToLower() == "localsubnet4")
            //    {
            //        firewallRule.RemoteAddresses = new IpCollection();
            //    }
            //    else if (powerShellResultList.ElementAt(0).Trim().ToLower() == "localsubnet6")
            //    {
            //        firewallRule.RemoteAddresses = new IpCollection();
            //    }
            //    else if (powerShellResultList.ElementAt(0).Trim().ToLower() == "playtodevice")
            //    {
            //        firewallRule.RemoteAddresses = new IpCollection();
            //    }
            //    else
            //    {
            //        List<String> powerShellResultIpList = powerShellResultList.ElementAt(0).ToLower().Replace("{", "").Replace("}", "").Split(",").ToList();
            //
            //        foreach (String powerShellResultIp in powerShellResultIpList)
            //        {
            //            IP ip = new IP(powerShellResultIp.Trim());
            //
            //            if (ip.Version != IpVersion.IP)
            //            {
            //                firewallRule.RemoteAddresses.Add(ip);
            //            }
            //        }
            //    }
            //}
        }

        public void WriteRemoteAddress()
        {
            DataRow dataRow;
            String sqlCommand;
            DynDnsIpAddressCollection firewallRuleNetworkAddressCollection;
            DynDnsIpAddressCollection serviceIpAddresseCollection;
            DynDnsIpAddress networkIpAddress;
            DateTime changeDate = DateTime.Now;

            // build List of network addresses
            firewallRuleNetworkAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            serviceIpAddresseCollection = new DynDnsIpAddressCollection(this.Configuration);

            firewallRuleNetworkAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsFirewallRule;
            firewallRuleNetworkAddressCollection.ReferenceId = this.FirewallRuleID;
            
            serviceIpAddresseCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_WriteRemoteAddress, "read assigned Services for firewall rule"));
            sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_WriteRemoteAddress_AssignedServices);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_WriteRemoteAddress, sqlCommand));
            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleID>", this.FirewallRuleID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_WriteRemoteAddress, sqlCommand));

            Int32 i = 0;
            while (true)
            {
                dataRow = this.Database.GetDataRow(sqlCommand, i);

                if (dataRow != null)
                {
                    serviceIpAddresseCollection.Clear();
                    serviceIpAddresseCollection.ReferenceId = Convert.ToInt32(dataRow[0].ToString());

                    serviceIpAddresseCollection.ReadIpAddressCollection(DynDnsIpAddressObject.ServiceDNS);

                    foreach (DynDnsIpAddress serviceIpAddress in serviceIpAddresseCollection)
                    {
                        networkIpAddress = serviceIpAddresseCollection.NewIpAddress();
                        networkIpAddress.IpAddressObject = DynDnsIpAddressObject.UpdatedIP;
                        networkIpAddress.ChangeDate = changeDate;
                        networkIpAddress.IpAddress = serviceIpAddress.NetworkAddress;

                        if (!firewallRuleNetworkAddressCollection.Contains(networkIpAddress))
                        {
                            firewallRuleNetworkAddressCollection.Add(networkIpAddress);
                        }
                    }
                }
                else
                {
                    break;
                }

                i++;
            }

            firewallRuleNetworkAddressCollection.Sort();
            firewallRuleNetworkAddressCollection.WriteIpAddressCollection();

            this.PowerShell.ExecuteCommand("Get-NetFirewallRule -Name \'" + this.Name + "\' | Get-NetFirewallAddressFilter | Set-NetFirewallAddressFilter -RemoteAddress " + firewallRuleNetworkAddressCollection.NetworkAddresses);
        }

        public void PrepareFirewallRuleToDisc()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_PrepareFirewallRuleToDisc, "prepare Database for FirewallRule"));

            String sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_PrepareFirewallRuleToDisc);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsFirewallRule_PrepareFirewallRuleToDisc, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleName>", this.Name);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_PrepareIpAddressToDisc, sqlCommand));

            this.Database.ExecuteCommand(sqlCommand);
        }

        public void WriteFirewallRuleToDisc()
        {
            this.PrepareFirewallRuleToDisc();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsFirewallRule_WriteFirewallRuleToDisc, "write FirewallRule " + this.Name + " to disc"));

            String sqlCommand = this.Database.GetCommand(Command.DynDnsFirewallRule_WriteFirewallRuleToDisc);

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

            this.LastWriteDate = DateTime.Now;

            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleLastSeenTimestamp>", ((DateTimeOffset)this.LastSeenDate.ToUniversalTime()).ToUnixTimeSeconds().ToString());
            sqlCommand = sqlCommand.Replace("<DynDnsFirewallRuleLastWriteTimestamp>", ((DateTimeOffset)this.LastWriteDate.ToUniversalTime()).ToUnixTimeSeconds().ToString());

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