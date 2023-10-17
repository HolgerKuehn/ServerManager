namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Net.Sockets;

    public class DynDnsServer : DynDnsService
    {
        private DynDnsDomainCollection domainCollection;

        public DynDnsServer(Configuration configuration, int dynDnsSerciceID) : base (configuration, dynDnsSerciceID)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "set public DNS-Server IP Addresses"));

            DynDnsIpAddress dynDnsIpAddress;

            dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "45.90.28.58");
            dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
            dynDnsIpAddress.PrefixLength = 32;
            this.IpAddressCollection.Add(dynDnsIpAddress);

            dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "2a07:a8c0::6d:cda2");
            dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
            dynDnsIpAddress.PrefixLength = 64;
            this.IpAddressCollection.Add(dynDnsIpAddress);


            this.DomainCollection = new DynDnsDomainCollection(this.Configuration);
            
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int domainID = 0;
            int serviceID = 0;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                domainID = Convert.ToInt32(dataRow[0].ToString());
                DynDnsDomain domain = new DynDnsDomain(this.Configuration, domainID, serviceID);
                this.DomainCollection.Add(domain);
            }
        }

        public DynDnsDomainCollection DomainCollection
        {
            get { return this.domainCollection; }
            set { this.domainCollection = value; }
        }

        public override void GetPublicIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_GetPublicIpAddress, "request Public IP for " + this.Name));

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

        public override void GetIpAddress()
        {
            // read public DNS IP 
            base.GetPublicIpAddress();

            // get actual public IP 
            this.GetPublicIpAddress();

            // get private IP
            DynDnsIpAddressCollection ipAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            if (Socket.OSSupportsIPv6)
            {
                ipAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));
                ipAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
            }

            if (Socket.OSSupportsIPv4)
            {
                ipAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
            }

            base.GetDnsIpAddress(ipAddressCollection);

            // invoke dependig objects
            foreach (DynDnsDomain domain in this.DomainCollection)
            {
                domain.GetIpAddress();
            }

            // write public IP to disc for all local services from server
            base.WriteIpAddressCollectionToDisc(Command.DynDnsServer_GetIpAddress, this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.Public));
        }

        public override void SetDnsServer()
        {
            foreach (DynDnsDomain domain in this.DomainCollection)
            {
                domain.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));
                domain.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
                domain.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
                domain.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));

                domain.SetDnsServer();
            }
        }
    }
}
