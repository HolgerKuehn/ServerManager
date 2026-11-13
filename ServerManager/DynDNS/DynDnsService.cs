namespace blog.dachs.ServerManager.DynDNS
{
    using blog.dachs.ServerManager;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public enum DynDnsServiceType : byte
    {
        Domain = 1,
        Server = 2,
        ServiceLocal = 3,
        ServiceNetwork = 4,
        ServiceRemote = 5
    }

    public abstract class DynDnsService : GlobalExtention
    {
        private Int32 dynDnsServiceID;
        private String name;
        private DynDnsIpAddressCollection ipAddressCollection;
        private DynDnsService parent;

        public DynDnsService(Configuration configuration, Int32 dynDnsServiceID) : base(configuration)
        {
            this.IpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            this.IpAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading DynDnsService properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = String.Empty;

            String sqlCommand = this.Database.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            String dynDnsServiceName = String.Empty;

            // iterate each Domain
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceName = dataRow[0].ToString();

                if (dynDnsServiceName != null)
                {
                    this.Name = dynDnsServiceName;
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsService_DynDnsService, "created DynDnsService with DynDnsService_Name = " + Name));
                }
            }
        }

        public Int32 DynDnsServiceID
        {
            get { return dynDnsServiceID; }
            set
            {
                dynDnsServiceID = value;
                IpAddressCollection.ReferenceId = value;
            }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public DynDnsIpAddressCollection IpAddressCollection
        {
            get { return ipAddressCollection; }
            set { ipAddressCollection = value; }
        }

        public DynDnsService Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public DynDnsIpAddressCollection NewIpAddressCollection()
        {
            DynDnsIpAddressCollection ipAddressCollection;
            ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            ipAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;
            ipAddressCollection.ReferenceId = this.DynDnsServiceID;

            return ipAddressCollection;
        }

        public DynDnsIpAddress NewIpAddress()
        {
            DynDnsIpAddress ipAddress;
            ipAddress = new DynDnsIpAddress(this.Configuration);
            ipAddress.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;
            ipAddress.ReferenceId = this.DynDnsServiceID;

            return ipAddress;
        }

        public virtual void GetIpAddress()
        {
            this.GetPublicIpAddress();
        }

        public virtual void SetDnsServer()
        {
        }

        public virtual DynDnsIpAddressCollection GetDnsIpAddress(DynDnsIpAddressCollection dnsServerCollection, List<DynDnsIpAddressType> dynDnsIpAddressTypes, DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + this.Name));

            String powerShellCommandOriginal;
            String powerShellCommandReplace;
            DynDnsIpAddressCollection dnsIpAddressCollection;
            DynDnsIpAddress ipAddress;
            ProcessOutput ipAddressList;
            String ipAddressAddress;
            String dnsErrorMessage;

            powerShellCommandOriginal = this.Database.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            //powerShellCommandOriginal = "(nslookup <DomainName> <DnsServer> | Select-String \" \" | Where-Object LineNumber -gt 4)";

            dnsIpAddressCollection = this.NewIpAddressCollection();

            for (Int32 dnsServerNumber = 0; dnsServerNumber < dnsServerCollection.Count; dnsServerNumber++)
            {
                powerShellCommandReplace = powerShellCommandOriginal;
                powerShellCommandReplace = powerShellCommandReplace.Replace("<DomainName>", this.Name);
                powerShellCommandReplace = powerShellCommandReplace.Replace("<DnsServer>", dnsServerCollection.ElementAt(dnsServerNumber).IpAddress);

                dnsIpAddressCollection = this.NewIpAddressCollection();
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_GetDnsIpAddress, powerShellCommandReplace));

                ipAddressList = this.PowerShell.ExecuteCommand(powerShellCommandReplace);
                dnsErrorMessage = this.CommandLine.GetProcessOutput(ipAddressList, 0, "ProcessOutput_Text like '%Dieser Vorgang wurde wegen Zeitüberschreitung zurückgegeben%'");

                // on error try next DNS-Server
                if (dnsErrorMessage != null)
                {
                    this.CommandLine.DeleteProcessOutput(ipAddressList);
                    continue;
                }

                Int32 i = 0;
                while (true)
                {
                    ipAddressAddress = this.CommandLine.GetProcessOutput(ipAddressList, i);

                    if (ipAddressAddress != null)
                    {
                        ipAddressAddress = ipAddressAddress.Replace("Addresses:", "");
                        ipAddressAddress = ipAddressAddress.Replace("Address:", "");
                        
                        ipAddress = dnsIpAddressCollection.NewIpAddress();
                        ipAddress.IpAddressObject = ipAddressObject;
                        ipAddress.IpAddress = ipAddressAddress.Trim();

                        // replace IP Interface
                         if (this.Name.Contains("yusousoochi") || this.Name.Contains("kuehn"))
                        {
                            if (ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                            {
                                ipAddress.IpAddress = ipAddress.IpAddress.Replace("127c:61ff:fe2f:542c", "215:5dff:fe16:104");
                            }
                        }

                        if (ipAddressAddress.Trim() != "" && dynDnsIpAddressTypes.Contains(ipAddress.IpAddressType))
                            dnsIpAddressCollection.Add(ipAddress);
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }

                this.CommandLine.DeleteProcessOutput(ipAddressList);
                dnsIpAddressCollection.WriteIpAddressCollection();

                break;
            }

            return dnsIpAddressCollection;
        }

        public virtual DynDnsIpAddressCollection GetPublicIpAddress(DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            List<DynDnsIpAddressType> dynDnsIpAddressTypes = [DynDnsIpAddressType.Public];

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP from DNS for " + this.Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.NewIpAddressCollection();

            dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public);
            
            return this.GetDnsIpAddress(dnsServerCollection, dynDnsIpAddressTypes, ipAddressObject);
        }

        public virtual DynDnsIpAddressCollection GetPrivateIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Private IP from DNS for " + Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.NewIpAddressCollection();

            List<DynDnsIpAddressType> dnsIpAddressTypes = [
                    DynDnsIpAddressType.UniqueLocal,
                    DynDnsIpAddressType.LinkLocal,
                    DynDnsIpAddressType.Private
                ];

            List<DynDnsIpAddressType> dynDnsIpAddressTypes = [
                    DynDnsIpAddressType.Private,
                    DynDnsIpAddressType.UniqueLocal,
                    DynDnsIpAddressType.LinkLocal
                ];

            dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, dnsIpAddressTypes);
            
            return this.GetDnsIpAddress(dnsServerCollection, dynDnsIpAddressTypes);
        }

        public virtual DynDnsIpAddressCollection GetDomainIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Private IP from DNS for " + Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.NewIpAddressCollection();

            List<DynDnsIpAddressType> dnsIpAddressTypes = [
                    DynDnsIpAddressType.UniqueLocal,
                    DynDnsIpAddressType.LinkLocal,
                    DynDnsIpAddressType.Private
                ];

            List<DynDnsIpAddressType> dynDnsIpAddressTypes = [
                    DynDnsIpAddressType.Private,
                    DynDnsIpAddressType.UniqueLocal,
                    DynDnsIpAddressType.LinkLocal
                ];

            dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, dnsIpAddressTypes);

            return this.GetDnsIpAddress(dnsServerCollection, dynDnsIpAddressTypes);
        }

        public virtual void UpdatePublicDnsIpAddress()
        {
        }

        public virtual void UpdatePublicDnsIpAddress(String updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
        }

        public virtual void UpdateLocalDomainAddress(DynDnsIpAddressCollection dnsIpAddressCollection)
        {
        //    // variables
        //    List<DynDnsIpAddressType> localDomainDnsServerTypes = [
        //        DynDnsIpAddressType.Private,
        //        DynDnsIpAddressType.UniqueLocal,
        //        DynDnsIpAddressType.LinkLocal
        //    ];

        //    List<DynDnsIpAddressType> allLocalDomainTypes = [
        //        DynDnsIpAddressType.Public,
        //        DynDnsIpAddressType.Private,
        //        DynDnsIpAddressType.UniqueLocal,
        //        DynDnsIpAddressType.LinkLocal
        //    ];

        //    String userDnsDomain;
        //    String powershellCommand;
        //    DynDnsIpAddressCollection domainDnsServer;
        //    DynDnsIpAddressCollection serviceDomainIpAddresses;
        //    DynDnsIpAddressCollection domainIpAddresses;
        //    //DynDnsIpAddressCollection serviceDnsIpAddresses;
        //    DynDnsIpAddressCollection dnsIpAddresses;

        //    // read IpAddresses from Domain-Controller
        //    userDnsDomain = Environment.GetEnvironmentVariable("userdnsdomain");

        //    if (userDnsDomain != null && this.Name.Contains(userDnsDomain, StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        domainDnsServer = this.Parent.NewIpAddressCollection();
        //        domainDnsServer.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, localDomainDnsServerTypes);

        //        serviceDomainIpAddresses = this.GetDnsIpAddress(domainDnsServer, allLocalDomainTypes);

        //        //serviceDnsIpAddresses = this.NewIpAddressCollection();
        //        //serviceDnsIpAddresses.ReadIpAddressCollection(DynDnsIpAddressObject.ServiceDNS, allLocalDomainTypes);

        //        for (byte i = 2; i <= 5; i++)
        //        {
        //            for (byte j = 2; j <= 3; j++)
        //            {
        //                domainIpAddresses = serviceDomainIpAddresses.Filter((DynDnsIpAddressType)i, (DynDnsIpAddressVersion)j);
        //                dnsIpAddresses = dnsIpAddressCollection.Filter((DynDnsIpAddressType)i, (DynDnsIpAddressVersion)j);

        //                if (dnsIpAddresses.IpAddresses != "" && domainIpAddresses.IpAddresses != dnsIpAddresses.IpAddresses)
        //                {
        //                    // remove old address-record
        //                    powershellCommand = String.Empty;
        //                    foreach (DynDnsIpAddress ipAddress in domainIpAddresses)
        //                    {
        //                        powershellCommand = "Remove-DnsServerResourceRecord -ComputerName \"" + userDnsDomain.ToLower() + "\" -ZoneName \"" + userDnsDomain.ToLower() + "\" -Name \"" + this.Name.Replace("." + userDnsDomain.ToLower(), "") + "\" -Force ";

        //                        if (ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
        //                        {
        //                            // IPv4
        //                            powershellCommand = powershellCommand + "-RRType \"A\" -RecordData \"" + ipAddress.IpAddress + "\"";
        //                        }
        //                        else if (ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
        //                        {
        //                            // IPv6
        //                            powershellCommand = powershellCommand + "-RRType \"AAAA\" -RecordData \"" + ipAddress.IpAddress + "\"";
        //                        }
        //                        else
        //                        {
        //                            powershellCommand = String.Empty;
        //                        }

        //                        this.PowerShell.ExecuteCommand(powershellCommand);
        //                    }


        //                    // add now address-record
        //                    powershellCommand = String.Empty;
        //                    foreach (DynDnsIpAddress ipAddress in dnsIpAddresses)
        //                    {
        //                        powershellCommand = "Add-DnsServerResourceRecord -ComputerName \"" + userDnsDomain.ToLower() + "\" -ZoneName \"" + userDnsDomain.ToLower() + "\" -Name \"" + this.Name.Replace("." + userDnsDomain.ToLower(), "") + "\" ";

        //                        if (ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
        //                        {
        //                            // IPv4
        //                            powershellCommand = powershellCommand + "-A -IPv4Address \"" + ipAddress.IpAddress + "\"";
        //                        }
        //                        else if (ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
        //                        {
        //                            // IPv6
        //                            powershellCommand = powershellCommand + "-AAAA -IPv6Address \"" + ipAddress.IpAddress + "\"";
        //                        }
        //                        else
        //                        {
        //                            powershellCommand = String.Empty;
        //                        }

        //                        this.PowerShell.ExecuteCommand(powershellCommand);
        //                    }
        //                }
        //            }
        //        }
        //    }
        }

        public virtual void WriteIpAddressHistory()
        {
            DynDnsIpAddressCollection currentIpAddressCollection;
            DynDnsIpAddressCollection historyIpAddressCollection;
            DynDnsIpAddress historyIpAddress;

            currentIpAddressCollection = this.NewIpAddressCollection();
            currentIpAddressCollection.ReadIpAddressCollection(DynDnsIpAddressObject.ServiceDNS);
            historyIpAddressCollection = this.NewIpAddressCollection();

            foreach (DynDnsIpAddress currentIpAddress in currentIpAddressCollection)
            {
                historyIpAddressCollection.Clear();
                historyIpAddressCollection.ReadIpAddressCollection(DynDnsIpAddressObject.UpdateHistory, currentIpAddress.IpAddressType, currentIpAddress.IpAddressVersion);

                currentIpAddress.Sorting = DynDnsIpAddressSorting.byChangeDateDesc;
                foreach (DynDnsIpAddress historyIpAddressSorting in historyIpAddressCollection)
                {
                    historyIpAddressSorting.Sorting = DynDnsIpAddressSorting.byChangeDateDesc;
                }

                historyIpAddressCollection.Sort();

                if (historyIpAddressCollection.Count > 0)
                {
                    historyIpAddress = historyIpAddressCollection.ElementAt(0);
                }
                else
                {
                    historyIpAddress = this.NewIpAddress();
                    historyIpAddress.IpAddress = "0";
                }

                if (currentIpAddress.IpAddress != historyIpAddress.IpAddress)
                {
                    currentIpAddress.IpAddressObject = DynDnsIpAddressObject.UpdateHistory;
                    historyIpAddressCollection.Insert(0, currentIpAddress);
                    historyIpAddressCollection.WriteIpAddressCollection();
                }
            }
        }
    }
}
