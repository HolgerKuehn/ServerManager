namespace blog.dachs.ServerManager
{
    public class LogDatabase : Log
    {
        public LogDatabase(Configuration configuration) : base(configuration)
        {
            this.Database = Database.GetHandlerDatabase(configuration);
        }

        public override void WriteLog(LogEntry logEntry)
        {
            if (this.Configuration.MinimumLogSeverity <= logEntry.LogSeverity)
            {
                String sqlInsertHeader = String.Empty;
                String sqlInsertValue = String.Empty;

                sqlInsertHeader += "insert into Log ";
                sqlInsertHeader += "   (";
                sqlInsertValue += "select ";

                sqlInsertHeader += "Log_Timestamp, ";
                sqlInsertValue += "unixepoch() as Log_Timestamp, ";

                sqlInsertHeader += "LogSeverity_ID, ";
                sqlInsertValue += ((Int32)logEntry.LogSeverity).ToString() + " as LogSeverity_ID, ";

                sqlInsertHeader += "LogOrigin_ID, ";
                sqlInsertValue += ((Int32)logEntry.LogOrigin).ToString() + " as LogOrigin_ID, ";

                sqlInsertHeader += "Log_Message";
                sqlInsertValue += "\'" + logEntry.LogMessage.Replace("\"", "''") + "\' as Log_Message";

                sqlInsertHeader += ") ";

                this.Database.ExecuteCommand(sqlInsertHeader + sqlInsertValue);
            }
        }
    }
}
