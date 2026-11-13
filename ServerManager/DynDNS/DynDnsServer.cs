namespace blog.dachs.ServerManager.DynDNS
{
    using System.Data;
    using System.Net;
    using System.Net.Sockets;

    public class DynDnsServer : DynDnsService
    {
        private DynDnsDomainCollection domainCollection;

        public DynDnsServer(Configuration configuration, Int32 dynDnsSerciceID) : base(configuration, dynDnsSerciceID)
        {
            this.DomainCollection = new DynDnsDomainCollection(this.Configuration);
            this.Parent = this;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            String sqlCommand = Database.GetCommand(Command.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            Int32 serviceID = 0;
            DynDnsDomain domain;

            // iterate each Domain
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
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

        public override DynDnsIpAddressCollection GetPublicIpAddress(DynDnsIpAddressObject ipAddressObject = DynDnsIpAddressObject.ServiceDNS)
        {
            DynDnsIpAddressCollection publicIpAddressCollection;
            DynDnsIpAddress publicIpAddress;
            String publicIP;

            publicIpAddressCollection = this.NewIpAddressCollection();


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_GetPublicIpAddress, "request Public IP for " + Name));

            if (Socket.OSSupportsIPv4)
            {
                publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv4);
                publicIpAddress = publicIpAddressCollection.NewIpAddress();
                publicIpAddress.IpAddressObject = ipAddressObject;
                publicIpAddress.IpAddress = publicIP.Trim();

                if (publicIP.Trim() != "")
                    publicIpAddressCollection.Add(publicIpAddress);
            }

            // IPv6 is determined from NetworkAdapter
            //if (Socket.OSSupportsIPv6)
            //{
            //    publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv6);
            //
            //    publicIpAddress = publicIpAddressCollection.NewIpAddress();
            //    publicIpAddress.IpAddressObject = ipAddressObject;
            //    publicIpAddress.IpAddress = publicIP.Trim();
            //
            //    if (publicIP.Trim() != "")
            //        publicIpAddressCollection.Add(publicIpAddress);
            //}

            publicIpAddressCollection.WriteIpAddressCollection();
            return publicIpAddressCollection;
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
                DynDnsIpAddress ipAddress;

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
                    ipAddress = publicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "45.90.28.58";
                    publicDnsServerCollection.Add(ipAddress);

                    ipAddress = publicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "45.90.30.58";
                    publicDnsServerCollection.Add(ipAddress);

                    ipAddress = publicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "2a07:a8c0::6d:cda2";
                    publicDnsServerCollection.Add(ipAddress);

                    ipAddress = publicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "2a07:a8c1::6d:cda2";
                    publicDnsServerCollection.Add(ipAddress);

                    privateDnsServerCollection.Add(publicDnsServerCollection);
                }

                domain.IpAddressCollection.Add(privateDnsServerCollection);
                domain.IpAddressCollection.WriteIpAddressCollection();
                domain.SetDnsServer();
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
