using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    using System.Data;
    using System.Net.Sockets;

    public class DynDnsServer : DynDnsService
    {
        private DynDnsDomainCollection domainCollection;

        public DynDnsServer(Configuration configuration, int dynDnsSerciceID) : base(configuration, dynDnsSerciceID)
        {
            DomainCollection = new DynDnsDomainCollection(Configuration);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            string sqlCommand = Database.GetCommand(Command.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", Configuration.ConfigurationID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int domainID = 0;
            int serviceID = 0;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                domainID = Convert.ToInt32(dataRow[0].ToString());
                serviceID = Convert.ToInt32(dataRow[1].ToString());
                DynDnsDomain domain = new DynDnsDomain(Configuration, domainID, serviceID);
                DomainCollection.Add(domain);
            }
        }

        public DynDnsDomainCollection DomainCollection
        {
            get { return domainCollection; }
            set { domainCollection = value; }
        }

        public override void GetPublicIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_GetPublicIpAddress, "request Public IP for " + Name));

            IpAddressCollection.Remove(DynDnsIpAddressType.Public);

            string publicIP = string.Empty;

            if (Socket.OSSupportsIPv4)
            {
                publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv4);
                IpAddressCollection.Add(publicIP.Trim());
            }

            if (Socket.OSSupportsIPv6)
            {
                publicIP = WebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv6);
                IpAddressCollection.Add(publicIP.Trim());
            }

            SetIpAddressPrefix();
        }

        public override void GetIpAddress()
        {
            // read public DNS IP 
            base.GetPublicIpAddress();

            // get actual public IP 
            GetPublicIpAddress();

            // get private IP
            DynDnsIpAddressCollection ipAddressCollection = new DynDnsIpAddressCollection(Configuration);
            ipAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;
            ipAddressCollection.ReferenceId = DynDnsServiceID;

            if (Socket.OSSupportsIPv6)
            {
                ipAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));
                ipAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
            }

            if (Socket.OSSupportsIPv4)
            {
                ipAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
            }

            base.GetDnsIpAddress(ipAddressCollection);

            // invoke dependig objects
            foreach (DynDnsDomain domain in DomainCollection)
            {
                domain.GetIpAddress();
            }

            // write public IP to disc for all local services from server
            WriteIpAddressCollectionToDisc(Command.DynDnsServer_GetIpAddress, IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.Public));
        }

        public override void SetDnsServer()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_SetDnsServer, "set public DNS-Server IP Addresses"));

            foreach (DynDnsDomain domain in DomainCollection)
            {
                DynDnsIpAddressCollection ipAddressCollection = domain.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic);
                if (ipAddressCollection.Count == 0)
                {
                    domain.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));
                }

                domain.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
                domain.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
                domain.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));

                domain.SetDnsServer();
            }
        }

        public override void WriteLogForChangedIpAddress()
        {
            base.WriteLogForChangedIpAddress();

            foreach (DynDnsDomain domain in DomainCollection)
            {
                domain.WriteLogForChangedIpAddress();
            }
        }

        public override void UpdatePublicDnsIpAddress()
        {
            base.UpdatePublicDnsIpAddress();

            foreach (DynDnsDomain domain in DomainCollection)
            {
                domain.UpdatePublicDnsIpAddress();
            }
        }
    }
}
