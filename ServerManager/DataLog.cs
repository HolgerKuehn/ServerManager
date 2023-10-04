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
        ServerManagerGuiMain_SystemTrayIcon_OnLogClicked = 3
    }

    internal class DataLog
    {
        private HandlerSqlite handlerSqlite;

        public DataLog()
        {
            this.handlerSqlite = new HandlerSqlite();
        }

        public void WriteLog(DataLogEntry dataLogEntry)
        {
            this.handlerSqlite.Command(dataLogEntry.DataLogInsert());
        }
    }
}
