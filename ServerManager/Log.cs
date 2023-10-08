using System.Data;
using System.Runtime.CompilerServices;

namespace blog.dachs.ServerManager
{
    public enum LogSeverity
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

    public enum LogOrigin
    {
        ProgramMain_Main = 1,
        GuiMain_GuiMain = 2,
        GuiMain_SystemTrayIcon_OnLogClicked = 3,
        GuiWindowLog_GuiWindowLog = 4,
        GuiWindowLog_GuiWindowLog_Shown = 5,
        GuiWindowLog_TmrGuiWindowLog_Tick = 6,
        GuiWindowLog_GuiWindowLog_VisibleChanged = 7,
        ThreadDynDns_ThreadDynDns = 8,
        DynDnsDomain_DynDnsDomain = 9,
        DynDnsServer_DynDnsServer = 10
    }

public class Log
    {
        private HandlerDatabase HandlerDatabase;
        private int configurationID;
        private LogSeverity minimumLogSeverity;

        public Log()
        {
            this.HandlerDatabase = HandlerDatabase.GetHandlerDatabase();

            string sqlCommand = this.HandlerDatabase.GetSqlCommand(DatabaseSqlCommand.Log_Log);
            sqlCommand = sqlCommand.Replace("<MachineName>", Environment.MachineName);

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow;

            for (int row = 0;  row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                this.ConfigurationID = Convert.ToInt32(dataRow[0]);
                this.MinimumLogSeverity = (LogSeverity) Convert.ToInt32(dataRow[1]);
            }
        }

        public LogSeverity MinimumLogSeverity
        {
            get { return this.minimumLogSeverity; }
            set { this.minimumLogSeverity = value; }
        }

        public int ConfigurationID
        {
            get { return this.configurationID; }
            set { this.configurationID = value; }
        }
        
        public void WriteLog(LogEntry logEntry)
        {
            if(this.MinimumLogSeverity <= logEntry.LogSeverity)
                this.HandlerDatabase.Command(logEntry.GetLogInsert());
        }
    }
}
