namespace blog.dachs.ServerManager
{
    using System.Data;
    
    public class ThreadDynDns : ThreadWorker
    {
        private DynDnsServer dynDnsServer;

        public ThreadDynDns(Configuration configuration) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading DynDnsServiceType"));
            
            string sqlCommand = this.Database.GetCommand(Command.ThreadDynDns_ThreadDynDns_DynDnsServiceType);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());
            
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand)); 

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            DynDnsServiceType dynDnsServiceType = DynDnsServiceType.Server;

            // get DynDnsServiceType for server 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceType = (DynDnsServiceType)Convert.ToInt32(dataRow[0].ToString());
            }


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading DynDnsServer"));

            sqlCommand = this.Database.GetCommand(Command.ThreadDynDns_ThreadDynDns_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            dataTable = this.Database.GetDataTable(sqlCommand);
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
                    dynDnsServer = new DynDnsServerLocal(configuration, dynDnsServiceID);
                    break;

                case DynDnsServiceType.ServiceRemote:
                    dynDnsServer = new DynDnsServerRemote(configuration, dynDnsServiceID);
                    break;
            }
        }

        public DynDnsServer DynDnsServer
        {
            get { return this.dynDnsServer; }
            set { this.dynDnsServer = value; }
        }

        public override void Work()
        {
            this.DynDnsServer.SetDnsServer();

            while (true)
            {
                this.DynDnsServer.GetIpAddress();
                this.DynDnsServer.WriteLogForChangedIpAddress();
                this.DynDnsServer.UpdatePublicDnsIpAddress();

                Thread.Sleep(120000);
            }
        }
    }
}
