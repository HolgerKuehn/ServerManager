namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Diagnostics;

    public class DynDnsServer : DynDnsNetworkObject
    {
        private DynDnsDomainCollection dynDnsDomains;

        public DynDnsServer(Configuration configuration, int dynDnsSerciceID) : base (configuration, dynDnsSerciceID)
        {
            this.DynDnsDomains = new DynDnsDomainCollection(this.Configuration);
            
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
        }

        public DynDnsDomainCollection DynDnsDomains
        {
            get { return this.dynDnsDomains; }
            set { this.dynDnsDomains = value; }
        }

        public void GetIpAddress()
        {
            base.GetDnsIpAddress();
            base.GetPublicIpAddress();
        }
    }
}
