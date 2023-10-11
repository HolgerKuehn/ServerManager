namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Net.Sockets;

    public abstract class DynDnsNetworkObject : GlobalExtention
    {
        private int dynDnsServiceID;
        private string name;
        private DynDnsDnsServerCollection dnsServerCollection;
        private DynDnsIpAddressCollection ipAddressCollection;

        public DynDnsNetworkObject(Configuration configuration, int dynDnsServiceID) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_DynDnsNetworkObject, "reading network object properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsNetworkObject_DynDnsNetworkObject);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_DynDnsNetworkObject, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsNetworkObjectName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsNetworkObjectName = dataRow[0].ToString();
                
                if (dynDnsNetworkObjectName != null)
                    this.Name = dynDnsNetworkObjectName;
            }

            this.DnsServerCollection = new DynDnsDnsServerCollection(this.Configuration);
            this.IpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            this.DnsServerCollection.Add(new DynDnsDnsServerPrivate(this.Configuration));
            this.DnsServerCollection.Add(new DynDnsDnsServerPublic(this.Configuration));

            this.ReadIpAddressCollectionFromDisc();
        }

        public int DynDnsServiceID
        {
            get { return this.dynDnsServiceID; }
            set { this.dynDnsServiceID = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public DynDnsDnsServerCollection DnsServerCollection
        {
            get { return this.dnsServerCollection; }
            set { this.dnsServerCollection = value; }
        }

        public DynDnsIpAddressCollection IpAddressCollection
        {
            get { return ipAddressCollection; }
            set { this.ipAddressCollection = value; }
        }

        public DynDnsDnsServer GetDnsServer(DnsServerType dnsServerType)
        {
            return this.DnsServerCollection.GetDnsServer(dnsServerType);
        }

        public virtual void GetDnsIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_GetDnsIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommand = this.HandlerDatabase.GetCommand(Command.DynDnsNetworkObject_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.name);
            powerShellCommand = powerShellCommand.Replace("<DnsServer>", this.GetDnsServer(DnsServerType.Private).GetDynDnsDnsServerIp().IpAddress);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_GetDnsIpAddress, powerShellCommand));

            List<string> ipAddressList = this.HandlerPowerShell.Command(powerShellCommand);

            foreach (string ipAddress in ipAddressList)
            {
                this.IpAddressCollection.Add(ipAddress.Trim());
            }

            this.SetIpAddressPrefix();
        }

        public virtual void GetPublicIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_GetPublicIpAddress, "request Public IP for " + this.Name));

            this.IpAddressCollection.Remove(DynDnsIpAddressType.Public);

            string publicIP = string.Empty;

            if (Socket.OSSupportsIPv4)
            {
                publicIP = this.HandlerWebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv4);
                this.IpAddressCollection.Add(publicIP.Trim());
            }

            if (Socket.OSSupportsIPv6)
            {
                publicIP = this.HandlerWebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv6);
                this.IpAddressCollection.Add(publicIP.Trim());
            }

            this.SetIpAddressPrefix();
        }

        private void SetIpAddressPrefix()
        {
            byte prefixLength;
            foreach (DynDnsIpAddress dynDnsIpAddress in this.IpAddressCollection)
            {
                if (dynDnsIpAddress.PrefixLength == 0)
                {
                    if (
                        dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.UniqueLocal ||
                        dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.LinkLocal ||
                        dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.Private
                    )
                    {
                        prefixLength = this.GetDnsServer(DnsServerType.Private).GetDynDnsDnsServerIp(dynDnsIpAddress.DynDnsIpAddressVersion).PrefixLength;
                    }
                    else if (dynDnsIpAddress.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv4)
                    {
                        prefixLength = 32;
                    }
                    else
                    {
                        prefixLength = 64;
                    }

                    dynDnsIpAddress.PrefixLength = prefixLength;
                }
            }

            this.IpAddressCollection.Remove(DynDnsIpAddressType.NotValid);
            this.WriteIpAddressCollectionToDisc();
        }

        private void ReadIpAddressCollectionFromDisc()
        {

        }

        private void WriteIpAddressCollectionToDisc()
        {

        }
    }
}
