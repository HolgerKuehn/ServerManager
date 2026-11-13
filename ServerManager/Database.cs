namespace blog.dachs.ServerManager
{
    using System.Data;

    public abstract class Database : GlobalExtention
    {
        protected Database(Configuration configuration) : base(configuration)
        {
        }

        public abstract void ExecuteCommand(String sqlCommand);

        public abstract String GetCommand(Command command);

        public abstract DataTable GetDataTable(String sqlCommand);

        public abstract DataRow? GetDataRow(String sqlCommand, Int32 row, String filter = "");

        public static Database GetHandlerDatabase(Configuration configuration)
        {
            return new Sqlite(configuration);
        }
    }
}