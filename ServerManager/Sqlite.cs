namespace blog.dachs.ServerManager
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    
    public class Sqlite : Database
    {
        private readonly DbConnection dbConnection;

        public Sqlite(Configuration configuration) : base(configuration)
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
            bool retry = true;
            SQLiteCommand sqliteCommand = new SQLiteCommand(sqlCommand, (SQLiteConnection)this.dbConnection);

            while (retry)
            {
                retry = false;

                try
                {
                    sqliteCommand.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    // database locked
                    if (ex.ErrorCode == 5)
                    { 
                        retry = true;
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        public override DataTable GetDataTable(string sqlCommand)
        {
            bool retry = true;
            DataTable dataTable = new DataTable();
            SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sqlCommand, (SQLiteConnection)this.dbConnection);


            while (retry)
            {
                retry = false;

                try
                {
                    sqliteDataAdapter.Fill(dataTable);
                }
                catch (SQLiteException ex)
                {
                    // database locked
                    if (ex.ErrorCode == 5)
                    {
                        retry = true;
                        Thread.Sleep(5000);
                    }
                }
            }

            return dataTable;
        }

        public override DataRow? GetDataRow(string sqlCommand, int row, string filter = "")
        {
            sqlCommand = sqlCommand + filter;
            sqlCommand = sqlCommand + " LIMIT 1 OFFSET " + row.ToString();
            
            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataRow = dataTable.Rows[i];
            }

            return dataRow;
        }
    }
}
