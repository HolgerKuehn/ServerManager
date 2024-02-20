namespace blog.dachs.ServerManager.DynDNS
{
    using blog.dachs.ServerManager;
    using System.Data;
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
        private int dynDnsServiceID;
        private string name;
        private DynDnsIpAddressCollection ipAddressCollection;
        private DynDnsService parent;

        public DynDnsService(Configuration configuration, int dynDnsServiceID) : base(configuration)
        {
            this.IpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            this.IpAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading DynDnsService properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.Database.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsServiceName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
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

        public int DynDnsServiceID
        {
            get { return dynDnsServiceID; }
            set
            {
                dynDnsServiceID = value;
                IpAddressCollection.ReferenceId = value;
            }
        }

        public string Name
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

        public virtual void GetDnsIpAddress(DynDnsIpAddressCollection dnsServerCollection, DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommandOriginal;
            string powerShellCommandReplace;
            DynDnsIpAddressCollection dnsIpAddressCollection;
            DynDnsIpAddress ipAddress;
            ProcessOutput ipAddressList;
            string ipAddressAddress;
            string dnsErrorMessage;

            powerShellCommandOriginal = this.Database.GetCommand(Command.DynDnsService_GetDnsIpAddress);

            for (int dnsServerNumber = 0; dnsServerNumber < dnsServerCollection.Count; dnsServerNumber++)
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

                int i = 3;
                while (true)
                {
                    ipAddressAddress = this.CommandLine.GetProcessOutput(ipAddressList, i);

                    if (ipAddressAddress != null)
                    {
                        ipAddress = new DynDnsIpAddress(this.Configuration);
                        ipAddress.IpAddressObject = ipAddressObject;
                        ipAddress.ReferenceType = dnsIpAddressCollection.ReferenceType;
                        ipAddress.ReferenceId = dnsIpAddressCollection.ReferenceId;
                        ipAddress.IpAddress = ipAddressAddress.Trim();

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
        }

        public virtual void GetPublicIpAddress(DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP from DNS for " + this.Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.NewIpAddressCollection();

            dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public);
            
            this.GetDnsIpAddress(dnsServerCollection, ipAddressObject);
        }

        public virtual void GetPrivateIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Private IP from DNS for " + Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.NewIpAddressCollection();

            List<DynDnsIpAddressType> dnsIpAddressTypes = [
                    DynDnsIpAddressType.UniqueLocal,
                    DynDnsIpAddressType.LinkLocal,
                    DynDnsIpAddressType.Private
                ];

            dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, dnsIpAddressTypes);
            
            this.GetDnsIpAddress(dnsServerCollection);
        }

        public virtual void UpdatePublicDnsIpAddress()
        {
        }

        public virtual void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
        }
    }
}
