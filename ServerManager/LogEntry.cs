namespace blog.dachs.ServerManager
{
    public class LogEntry
    {
        private LogSeverity logSeverity = LogSeverity.Informational;
        private LogOrigin logOrigin     = LogOrigin.ProgramMain_Main;
        private String logMessage       = String.Empty;

        public LogEntry(LogSeverity logSeverity, LogOrigin logOrigin, String logMessage)
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

        public String LogMessage
        {
            get { return this.logMessage; }
            set { this.logMessage = value; }
        }
    }
}
