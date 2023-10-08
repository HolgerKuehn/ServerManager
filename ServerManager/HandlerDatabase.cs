namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Data.Common;

    public enum DatabaseSqlCommand
    {
        Log_Log = 1,
        ThreadDynDns_ThreadDynDns_DynDnsServiceType = 2,
        ThreadDynDns_ThreadDynDns_DynDnsServer = 3,
        DynDnsDomain_DynDnsDomain = 4,
        DynDnsServer_DynDnsServer = 5
    }

    public abstract class HandlerDatabase
    {
        private readonly DbConnection dbConnection;

        public abstract void Command(string sqlCommand);

        public abstract string GetSqlCommand(DatabaseSqlCommand databaseSqlCommand);

        public abstract DataTable GetDataTable(string sqlCommand);

        public static HandlerDatabase GetHandlerDatabase()
        {
            return new HandlerSqlite();
        }
    }
}