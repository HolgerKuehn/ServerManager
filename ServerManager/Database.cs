namespace blog.dachs.ServerManager
{
    using System.Data;

    public abstract class Database : GlobalExtention
    {
        protected Database(Configuration configuration) : base(configuration)
        {
        }

        public abstract void ExecuteCommand(string sqlCommand);

        public abstract string GetCommand(Command command);

        public abstract DataTable GetDataTable(string sqlCommand);

        public abstract DataRow? GetDataRow(string sqlCommand, int row, string filter = "");

        public static Database GetHandlerDatabase(Configuration configuration)
        {
            return new Sqlite(configuration);
        }
    }
}