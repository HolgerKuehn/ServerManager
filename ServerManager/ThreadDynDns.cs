using System.Data;
using System.Text;

namespace blog.dachs.ServerManager
{
    public class ThreadDynDns : ThreadWorker
    {
        private Log log;
        private HandlerSqlite handlerSqlite;
        private DynDnsDomainCollection dynDnsDomains;

        public ThreadDynDns(Log log)
        {
            this.Log = log;
            this.HandlerSqlite = new HandlerSqlite();
            this.DynDnsDomains = new DynDnsDomainCollection();

            string sqlCommand = string.Empty;

            sqlCommand += "select a.DynDnsDomain_ID ";
            sqlCommand += "from DynDnsDomain as a ";
            sqlCommand += "where a.Configuration_ID = " + this.Log.ConfigurationID.ToString();

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading Domains"));
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand)); 
            DataTable dataTable = this.HandlerSqlite.GetDataTable(sqlCommand);
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
        }

        public Log Log
        {
            get { return this.log; }
            set { this.log = value; }
        }

        public HandlerSqlite HandlerSqlite
        {
            get { return this.handlerSqlite; }
            set { this.handlerSqlite = value; }
        }

        public DynDnsDomainCollection DynDnsDomains
        {
            get { return this.dynDnsDomains; }
            set { this.dynDnsDomains = value; }
        }

        public override void Work()
        {
            while (true)
            {
                foreach (DynDnsDomain dynDnsDomain in this.DynDnsDomains)
                {
                
                }

                Thread.Sleep(120000);
            }
        }
    }
}
