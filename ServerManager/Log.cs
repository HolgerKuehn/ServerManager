namespace blog.dachs.ServerManager
{
    using System.Data;

    public enum LogType
    {
        File = 1,
        Database = 2
    }

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
        DynDnsServer_DynDnsServer = 10,
        DynDnsNetworkObject_DynDnsNetworkObject = 11,
        DynDnsNetworkObject_GetIpAddress = 12
    }

    public abstract class Log : GlobalExtention
    {
        public Log(Configuration configuration) : base(configuration)
        {
        }

        public abstract void WriteLog(LogEntry logEntry);
    }
}
