namespace blog.dachs.ServerManager
{
    using System.Configuration;
    using System.Data;

    public class Configuration
    {
        private int configurationID;
        private Dictionary<LogType, Log> logs;
        private LogSeverity minimumLogSeverity;
        ThreadCollection threadCollection;
        private HandlerDatabase handlerDatabase;


        public Configuration()
        {
            this.Logs = new Dictionary<LogType, Log>();
            this.Logs[LogType.File] = new LogFile(this);
            this.Logs[LogType.Database] = new LogDatabase(this);

            this.ThreadCollection = new ThreadCollection(this);

            this.HandlerDatabase = HandlerDatabase.GetHandlerDatabase(this);

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.Configuration_Configuration);
            sqlCommand = sqlCommand.Replace("<MachineName>", Environment.MachineName);

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow;

            if (dataTable.Rows.Count != 0)
            {
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    dataRow = dataTable.Rows[row];
                    this.ConfigurationID = Convert.ToInt32(dataRow[0]);
                    this.MinimumLogSeverity = (LogSeverity)Convert.ToInt32(dataRow[1]);
                }
            }
            else
            {
                string message = "No Configuration for MachineName \"" + Environment.MachineName + "\" found";
                string title = "Missing Configuration";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
        }

        private Dictionary<LogType, Log> Logs
        {
            get { return this.logs; }
            set { this.logs = value; }
        }

        public Log GetLog(LogType logType = LogType.Database)
        {
            return this.Logs[logType];
        }

        public HandlerDatabase HandlerDatabase
        {
            get { return this.handlerDatabase; }
            set { this.handlerDatabase = value; }
        }

        public int ConfigurationID
        {
            get { return this.configurationID; }
            set { this.configurationID = value; }
        }

        public LogSeverity MinimumLogSeverity
        {
            get { return this.minimumLogSeverity; }
            set { this.minimumLogSeverity = value; }
        }

        public ThreadCollection ThreadCollection
        {
            get { return this.threadCollection; }
            set { this.threadCollection = value; }
        }
    }
}
