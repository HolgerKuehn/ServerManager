namespace blog.dachs.ServerManager
{
    using System.Data;

    public class DynDnsServer
    {
        private Log log;
        private HandlerDatabase handlerDatabase;

        private string name;
        private DynDnsDomainCollection dynDnsDomains;
        private DynDnsDnsServerCollection dynDnsDnsServers;

        public DynDnsServer(Log log, int dynDnsSerciceID)
        {
            this.Log = log;
            this.HandlerDatabase = HandlerDatabase.GetHandlerDatabase();
            this.DynDnsDomains = new DynDnsDomainCollection();
            this.DynDnsDnsServers = new DynDnsDnsServerCollection();

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServer_DynDnsServer, "reading Domains"));

            string sqlCommand = this.HandlerDatabase.GetSqlCommand(DatabaseSqlCommand.DynDnsServer_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Log.ConfigurationID.ToString());

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            int dynDnsDomainID = 0;
            DynDnsDomain dynDnsDomain = null;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsDomainID = Convert.ToInt32(dataRow[0].ToString());
                dynDnsDomain = new DynDnsDomain(this.Log, dynDnsDomainID);
                this.DynDnsDomains.Add(dynDnsDomain);
            }

            this.DynDnsDnsServers.Add(new DynDnsDnsServerPrivate());
            this.DynDnsDnsServers.Add(new DynDnsDnsServerPublic());
        }

        public Log Log
        {
            get { return this.log; }
            set { this.log = value; }
        }

        public HandlerDatabase HandlerDatabase
        {
            get { return this.handlerDatabase; }
            set { this.handlerDatabase = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public DynDnsDomainCollection DynDnsDomains
        {
            get { return this.dynDnsDomains; }
            set { this.dynDnsDomains = value; }
        }

        public DynDnsDnsServerCollection DynDnsDnsServers
        {
            get { return this.dynDnsDnsServers; }
            set { this.dynDnsDnsServers = value; }
        }
    }
}
