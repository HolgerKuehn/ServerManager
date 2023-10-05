namespace blog.dachs.ServerManager
{
    public enum DataLogSeverity
    {
        Emergency = 128,
        Alert = 64,
        Critical = 32,
        Error = 16,
        Warning = 8,
        Notice = 4,
        Informational = 2,
        Debug = 1
    }

    public enum DataLogOrigin
    {
        ProgramMain_Main = 1,
        ServerManagerGuiMain_ServerManagerGuiMain = 2,
        ServerManagerGuiMain_SystemTrayIcon_OnLogClicked = 3,
        ServerManagerGuiWindowLog_ServerManagerGuiWindowLog = 4,
        ServerManagerGuiWindowLog_WindowLog_Shown = 5,
        ServerManagerGuiWindowLog_tmrServerManagerGuiWindowLog_Tick = 6
        ServerManagerGuiWindowLog_VisibleChanged
    }

public class DataLog
    {
        private HandlerSqlite handlerSqlite;
        private DataLogSeverity minimumDataLogSeverity;

        public DataLog()
        {
            this.handlerSqlite = new HandlerSqlite();

            string sqlCommand = string.Empty;

            sqlCommand += "select a.Configuration_MinimumDataLogSeverity ";
            sqlCommand += "from Configuration as a ";
            sqlCommand += "where a.Configuration_ServerName = \"" + Environment.MachineName + "\" ";

            minimumDataLogSeverity = (DataLogSeverity) Convert.ToInt32(this.handlerSqlite.GetDataTable(sqlCommand).Rows[0][0]);
        }

        public DataLogSeverity MinimumDataLogSeverity
        {
            get { return this.minimumDataLogSeverity; }
            set { this.minimumDataLogSeverity = value; }
        }

        public void WriteLog(DataLogEntry dataLogEntry)
        {
            if(this.MinimumDataLogSeverity <= dataLogEntry.DataLogSeverity)
                this.handlerSqlite.Command(dataLogEntry.DataLogInsert());
        }
    }
}
