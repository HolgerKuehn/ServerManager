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
        GuiMain_GuiMain = 2,
        GuiMain_SystemTrayIcon_OnLogClicked = 3,
        GuiWindowLog_GuiWindowLog = 4,
        GuiWindowLog_GuiWindowLog_Shown = 5,
        GuiWindowLog_tmrGuiWindowLog_Tick = 6,
        GuiWindowLog_GuiWindowLog_VisibleChanged = 7
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
