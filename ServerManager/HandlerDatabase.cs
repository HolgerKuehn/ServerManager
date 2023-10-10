namespace blog.dachs.ServerManager
{
    using System.Data;

    public abstract class HandlerDatabase
    {
        public abstract void ExecuteCommand(string sqlCommand);

        public abstract string GetCommand(Command command);

        public abstract DataTable GetDataTable(string sqlCommand);

        public static HandlerDatabase GetHandlerDatabase()
        {
            return new HandlerSqlite();
        }
    }
}