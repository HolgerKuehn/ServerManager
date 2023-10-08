namespace blog.dachs.ServerManager
{
    using System.Data;
    
    public class ThreadDynDns : ThreadWorker
    {
        private Log log;
        private HandlerDatabase handlerDatabase;
        private DynDnsServer dynDnsServer;

        public ThreadDynDns(Log log)
        {
            this.Log = log;
            this.HandlerDatabase = HandlerDatabase.GetHandlerDatabase();

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading DynDnsServiceType"));
            
            string sqlCommand = this.HandlerDatabase.GetSqlCommand(DatabaseSqlCommand.ThreadDynDns_ThreadDynDns_DynDnsServiceType);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Log.ConfigurationID.ToString());
            
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand)); 

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            DynDnsServiceType dynDnsServiceType = DynDnsServiceType.Server;

            // get DynDnsServiceType for server 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceType = (DynDnsServiceType)Convert.ToInt32(dataRow[0].ToString());
            }


            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading DynDnsServer"));

            sqlCommand = this.HandlerDatabase.GetSqlCommand(DatabaseSqlCommand.ThreadDynDns_ThreadDynDns_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Log.ConfigurationID.ToString());

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            dataRow = null;
            int dynDnsServiceID = 0;

            // get DynDnsServiceID for server 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceID = Convert.ToInt32(dataRow[0].ToString());
            }

            // create Server accordingly to it
            switch (dynDnsServiceType)
            {
                case DynDnsServiceType.ServiceLocal:
                    dynDnsServer = new DynDnsServerLocal(log, dynDnsServiceID);
                    break;

                case DynDnsServiceType.ServiceRemote:
                    dynDnsServer = new DynDnsServerRemote(log, dynDnsServiceID);
                    break;
            }
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

        public DynDnsServer DynDnsServer
        {
            get { return this.dynDnsServer; }
            set { this.dynDnsServer = value; }
        }

        public override void Work()
        {
            while (true)
            {
                

                Thread.Sleep(120000);
            }
        }
    }
}
