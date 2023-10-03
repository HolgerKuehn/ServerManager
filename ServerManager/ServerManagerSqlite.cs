using System.Data;
using System.Data.SQLite;

namespace blog.dachs.ServerManager
{
    public class ServerManagerSqlite
    {
        private readonly SQLiteConnection serverManagerSqliteConnection;

        public ServerManagerSqlite()
        {
            string connectionString = "Data Source=" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "blog.dachs", "ServerManager", "ServerManager.db");
            serverManagerSqliteConnection = new SQLiteConnection(connectionString);
            
            try
            {
                serverManagerSqliteConnection.Open();
            }
            catch (Exception ex)
            {

            }
        }

        public DataTable GetDataTable(string sqlCommand)
        {
            DataTable dataTable = new DataTable();
            SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sqlCommand, serverManagerSqliteConnection);
            sqliteDataAdapter.Fill(dataTable);

            return dataTable;
        }
    }
}
