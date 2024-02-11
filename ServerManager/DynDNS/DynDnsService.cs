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

        public DynDnsIpAddressCollection GetIpAddressCollection()
        {
            DynDnsIpAddressCollection ipAddressCollection;
            ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            ipAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;
            ipAddressCollection.ReferenceId = this.DynDnsServiceID;

            return ipAddressCollection;
        }

        public virtual void GetIpAddress()
        {
            this.GetPublicIpAddress();
        }

        public virtual void SetDnsServer()
        {
        }

        public virtual void GetDnsIpAddress(string dnsServer, DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommand;
            DynDnsIpAddressCollection dnsIpAddressCollection;
            DynDnsIpAddress ipAddress;
            ProcessOutput ipAddressList;

            powerShellCommand = this.Database.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.Name);
            powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsServer);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_GetDnsIpAddress, powerShellCommand));

            dnsIpAddressCollection = this.GetIpAddressCollection();
            ipAddressList = this.PowerShell.ExecuteCommand(powerShellCommand);

            int i = 3;
            while (true)
            {
                string ipAddressAddress = this.CommandLine.GetProcessOutput(ipAddressList, i);

                if (ipAddressAddress != null)
                {
                    ipAddress = new DynDnsIpAddress(this.Configuration, ipAddressAddress.Trim());
                    ipAddress.IpAddressObject = ipAddressObject;

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
        }

        public virtual void GetPublicIpAddress(DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP from DNS for " + this.Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.GetIpAddressCollection();

            if (Socket.OSSupportsIPv6)
            {
                dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public, DynDnsIpAddressVersion.IPv6);
            }
            else
            {
                dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public, DynDnsIpAddressVersion.IPv4);
            }

            if (dnsServerCollection.Count != 0)
            {
                this.GetDnsIpAddress(dnsServerCollection.ElementAt(0).IpAddress, ipAddressObject);
            }
        }

        public virtual void GetPrivateIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Private IP from DNS for " + Name));

            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = this.GetIpAddressCollection();

            if (Socket.OSSupportsIPv6)
            {
                dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.UniqueLocal, DynDnsIpAddressVersion.IPv6);
                dnsServerCollection.Remove(DynDnsIpAddressType.NotValid);

                if (dnsServerCollection.Count == 0)
                {
                    dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.LinkLocal, DynDnsIpAddressVersion.IPv6);
                    dnsServerCollection.Remove(DynDnsIpAddressType.NotValid);
                }
            }
            else
            {
                dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Private, DynDnsIpAddressVersion.IPv4);
            }

            this.GetDnsIpAddress(dnsServerCollection.ElementAt(0).IpAddress);
        }

        public virtual void UpdatePublicDnsIpAddress()
        {
        }

        public virtual void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
        }
    }
}
