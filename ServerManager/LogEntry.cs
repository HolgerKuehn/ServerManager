namespace blog.dachs.ServerManager
{
    public class LogEntry
    {
        private LogSeverity logSeverity = LogSeverity.Informational;
        private LogOrigin logOrigin     = LogOrigin.ProgramMain_Main;
        private string logMessage       = string.Empty;

        public LogEntry(LogSeverity logSeverity, LogOrigin logOrigin, string logMessage)
        {
            LogSeverity = logSeverity;
            LogOrigin   = logOrigin;
            LogMessage  = logMessage;
        }

        public LogSeverity LogSeverity
        {
            get { return this.logSeverity; }
            set { this.logSeverity = value; }
        }

        public LogOrigin LogOrigin
        {
            get { return this.logOrigin; }
            set { this.logOrigin = value; }
        }

        public string LogMessage
        {
            get { return this.logMessage; }
            set { this.logMessage = value; }
        }

        public string GetLogInsert()
        { 
            string sqlInsertHeader = string.Empty;
            string sqlInsertValue = string.Empty;

            sqlInsertHeader += "insert into Log ";
            sqlInsertHeader += "   (";
            sqlInsertValue  += "select ";

            sqlInsertHeader +=                "Log_Timestamp, ";
            sqlInsertValue  += "unixepoch() as Log_Timestamp, ";

            sqlInsertHeader +=                                          "LogSeverity_ID, ";
            sqlInsertValue  += ((int)this.logSeverity).ToString() + " as LogSeverity_ID, ";

            sqlInsertHeader +=                                        "LogOrigin_ID, ";
            sqlInsertValue  += ((int)this.LogOrigin).ToString() + " as LogOrigin_ID, ";

            sqlInsertHeader +=                                "Log_Message";
            sqlInsertValue  += "\"" + this.LogMessage + "\" as Log_Message";

            sqlInsertHeader += ") ";

            return sqlInsertHeader + sqlInsertValue;
        }
    }
}
