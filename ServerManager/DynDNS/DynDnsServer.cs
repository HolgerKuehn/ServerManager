namespace blog.dachs.ServerManager.DynDNS
{
    using System.Data;
    using System.Net.Sockets;

    public class DynDnsServer : DynDnsService
    {
        private DynDnsDomainCollection domainCollection;

        public DynDnsServer(Configuration configuration, int dynDnsSerciceID) : base(configuration, dynDnsSerciceID)
        {
            this.DomainCollection = new DynDnsDomainCollection(this.Configuration);
            this.Parent = this;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            string sqlCommand = Database.GetCommand(Command.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int serviceID = 0;
            DynDnsDomain domain;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                serviceID = Convert.ToInt32(dataRow[0].ToString());
                domain = new DynDnsDomain(Configuration, serviceID);
                domain.Parent = this;

                this.DomainCollection.Add(domain);
            }
        }

        public DynDnsDomainCollection DomainCollection
        {
            get { return domainCollection; }
            set { domainCollection = value; }
        }

        public override void GetPublicIpAddress(DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            DynDnsIpAddressCollection publicIpAddressCollection;
            DynDnsIpAddress publicIpAddress;
            string publicIP;

            publicIpAddressCollection = this.NewIpAddressCollection();


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_GetPublicIpAddress, "request Public IP for " + Name));

            if (Socket.OSSupportsIPv4)
            {
                publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv4);
                publicIpAddress = publicIpAddressCollection.NewIpAddress();
                publicIpAddress.IpAddressObject = ipAddressObject;
                publicIpAddress.IpAddress = publicIP.Trim();

                publicIpAddressCollection.Add(publicIpAddress);
            }

            if (Socket.OSSupportsIPv6)
            {
                publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv6);

                publicIpAddress = publicIpAddressCollection.NewIpAddress();
                publicIpAddress.IpAddressObject = ipAddressObject;
                publicIpAddress.IpAddress = publicIP.Trim();

                publicIpAddressCollection.Add(publicIpAddress);
            }

            publicIpAddressCollection.WriteIpAddressCollection();
        }

        public override void GetIpAddress()
        {
            // invoke depending objects
            foreach (DynDnsDomain domain in this.DomainCollection)
            {
                domain.GetIpAddress();
            }
        }

        public override void SetDnsServer()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_SetDnsServer, "set public DNS-Server IP Addresses"));

            foreach (DynDnsDomain domain in DomainCollection)
            {
                DynDnsIpAddressCollection publicDnsServerCollection = this.NewIpAddressCollection();
                DynDnsIpAddressCollection privateDnsServerCollection = this.NewIpAddressCollection();

                List<DynDnsIpAddressType> dynDnsIpAddressTypes = [
                    DynDnsIpAddressType.Private,
                    DynDnsIpAddressType.LinkLocal,
                    DynDnsIpAddressType.UniqueLocal,
                ];

                publicDnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public);
                privateDnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, dynDnsIpAddressTypes);

                publicDnsServerCollection.Remove(DynDnsIpAddressType.NotValid);
                privateDnsServerCollection.Remove(DynDnsIpAddressType.NotValid);

                if (publicDnsServerCollection.Count == 0)
                {
                    publicDnsServerCollection.Add(privateDnsServerCollection);
                }

                domain.IpAddressCollection.Add(privateDnsServerCollection);

                domain.SetDnsServer();
                domain.IpAddressCollection.WriteIpAddressCollection();
            }
        }

        public override void WriteIpAddressHistory()
        {
            base.WriteIpAddressHistory();

            // invoke depending objects
            foreach (DynDnsDomain domain in this.DomainCollection)
            {
                domain.WriteIpAddressHistory();
            }
        }
    }
}
