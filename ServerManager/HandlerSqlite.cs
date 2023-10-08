namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    
    public class HandlerSqlite : HandlerDatabase
    {
        private readonly DbConnection dbConnection;

        public HandlerSqlite()
        {
            string connectionString = "Data Source=" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "blog.dachs", "ServerManager", "ServerManager.db");
            dbConnection = new SQLiteConnection(connectionString);
            
            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {

            }
        }

        public override void Command(string sqlCommand)
        {
            SQLiteCommand sqliteCommand = new SQLiteCommand(sqlCommand, (SQLiteConnection)this.dbConnection);
            sqliteCommand.ExecuteNonQuery();
        }

        public override string GetSqlCommand(DatabaseSqlCommand databaseSqlCommand)
        {
            string sqlCommand = string.Empty;

            sqlCommand += "select a.DatabaseSqlCommand_Name ";
            sqlCommand += "from DatabaseSqlCommand a ";
            sqlCommand += "where a.DatabaseSqlCommand_ID = " + Convert.ToString((int)databaseSqlCommand) + " ";

            DataTable dataTable = this.GetDataTable(sqlCommand);
            DataRow dataRow;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                sqlCommand = Convert.ToString(dataRow[0]);
            }

            if (sqlCommand == null)
                sqlCommand = string.Empty;

            return sqlCommand;
        }

        public override DataTable GetDataTable(string sqlCommand)
        {
            DataTable dataTable = new DataTable();
            SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sqlCommand, (SQLiteConnection)this.dbConnection);
            sqliteDataAdapter.Fill(dataTable);

            return dataTable;
        }
    }
}
