namespace blog.dachs.ServerManager
{
    using System.Data;

    public class DynDnsServer : DynDnsNetworkObject
    {
        private DynDnsDomainCollection dynDnsDomains;
        private DynDnsDnsServerCollection dynDnsDnsServers;

        public DynDnsServer(Configuration configuration, int dynDnsSerciceID) : base (configuration, dynDnsSerciceID)
        {
            this.DynDnsDomains = new DynDnsDomainCollection(this.Configuration);
            this.DynDnsDnsServers = new DynDnsDnsServerCollection(this.Configuration);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int dynDnsDomainID = 0;
            DynDnsDomain dynDnsDomain = null;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsDomainID = Convert.ToInt32(dataRow[0].ToString());
                dynDnsDomain = new DynDnsDomain(this.Configuration, dynDnsDomainID);
                this.DynDnsDomains.Add(dynDnsDomain);
            }

            this.DynDnsDnsServers.Add(new DynDnsDnsServerPrivate(this.Configuration));
            this.DynDnsDnsServers.Add(new DynDnsDnsServerPublic(this.Configuration));
        }

        public DynDnsDomainCollection DynDnsDomains
        {
            get { return this.dynDnsDomains; }
            set { this.dynDnsDomains = value; }
        }

        private DynDnsDnsServerCollection DynDnsDnsServers
        {
            get { return this.dynDnsDnsServers; }
            set { this.dynDnsDnsServers = value; }
        }

        public DynDnsDnsServer GetDynDnsDnsServer(DynDnsDnsServerType dynDnsDnsServerType)
        {
            return this.DynDnsDnsServers.GetDynDnsDnsServer(dynDnsDnsServerType);
        }

        public void GetIpAddress()
        {
            DynDnsIpAddressCollection dynDnsIpAddressCollection;
            byte prefixLength;

            dynDnsIpAddressCollection = base.GetIpAddress(this.GetDynDnsDnsServer(DynDnsDnsServerType.Private));

            foreach(DynDnsIpAddress dynDnsIpAddress in dynDnsIpAddressCollection)
            {
                if (
                    dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.UniqueLocal ||
                    dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.LinkLocal ||
                    dynDnsIpAddress.DynDnsIpAddressType == DynDnsIpAddressType.Private
                )
                {
                    prefixLength = this.GetDynDnsDnsServer(DynDnsDnsServerType.Private).GetDynDnsDnsServerIp(dynDnsIpAddress.DynDnsIpAddressVersion).PrefixLength;
                }
                else if (dynDnsIpAddress.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv4)
                {
                    prefixLength = 32;
                }
                else
                {
                    prefixLength = 128;
                }

                dynDnsIpAddress.PrefixLength = prefixLength;
            }

            // save to database
        }
    }
}
