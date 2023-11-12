namespace blog.dachs.ServerManager
{
    public class LogDatabase : Log
    {
        public LogDatabase(Configuration configuration) : base(configuration)
        {
            this.HandlerDatabase = Database.GetHandlerDatabase(configuration);
        }

        public override void WriteLog(LogEntry logEntry)
        {
            if (this.Configuration.MinimumLogSeverity <= logEntry.LogSeverity)
            {
                string sqlInsertHeader = string.Empty;
                string sqlInsertValue = string.Empty;

                sqlInsertHeader += "insert into Log ";
                sqlInsertHeader += "   (";
                sqlInsertValue += "select ";

                sqlInsertHeader += "Log_Timestamp, ";
                sqlInsertValue += "unixepoch() as Log_Timestamp, ";

                sqlInsertHeader += "LogSeverity_ID, ";
                sqlInsertValue += ((int)logEntry.LogSeverity).ToString() + " as LogSeverity_ID, ";

                sqlInsertHeader += "LogOrigin_ID, ";
                sqlInsertValue += ((int)logEntry.LogOrigin).ToString() + " as LogOrigin_ID, ";

                sqlInsertHeader += "Log_Message";
                sqlInsertValue += "\"" + logEntry.LogMessage.Replace("\"", "'") + "\" as Log_Message";

                sqlInsertHeader += ") ";

                this.HandlerDatabase.ExecuteCommand(sqlInsertHeader + sqlInsertValue);
            }
        }
    }
}
