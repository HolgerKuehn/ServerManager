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
        public override string GetCommand(Command command)
        {
            string sqlCommand = string.Empty;

            sqlCommand += "select a.Command_Name ";
            sqlCommand += "from Command a ";
            sqlCommand += "where a.Command_ID = " + ((int)command).ToString() + " ";

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

        public override void ExecuteCommand(string sqlCommand)
        {
            SQLiteCommand sqliteCommand = new SQLiteCommand(sqlCommand, (SQLiteConnection)this.dbConnection);
            sqliteCommand.ExecuteNonQuery();
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
