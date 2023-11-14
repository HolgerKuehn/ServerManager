namespace blog.dachs.ServerManager
{
    public enum LogType : byte
    {
        File = 1,
        Database = 2
    }

    public enum LogSeverity : byte
    {
        SQL = 1,
        Debug = 2,
        Informational = 3,
        Notice = 4,
        Warning = 5,
        Error = 6,
        Critical = 7,
        Alert = 8,
        Emergency = 9
    }

    public enum LogOrigin : byte
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
        DynDnsServer_GetPublicIpAddress = 17,
        DynDnsServer_SetDnsServer = 21,
        DynDnsServerLocal_DynDnsServerLocal = 16,
        DynDnsService_DynDnsService = 11,
        DynDnsService_GetDnsIpAddress = 12,
        DynDnsService_GetPublicIpAddress = 13,
        DynDnsService_PrepareIpAddressCollectionToDisc = 14,
        DynDnsService_ReadIpAddressCollectionFromDisc = 22,
        DynDnsService_UpdatePublicDnsIpAddress = 18,
        DynDnsService_WriteIpAddressCollectionToDisc = 15,
        DynDnsService_WriteLogForChangedIpAddress = 20,
        DynDnsServiceLocal_UpdatePublicDnsIpAddress = 19,
        DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell = 23,
        DynDnsFirewallRule_DynDnsFirewallRule = 24,
        DynDnsFirewallRule_ReadFirewallRuleBaseProperties = 25,
        DynDnsIpCollection_ReadIpAddressCollectionFromDisc = 26,
        ThreadBackup_ThreadBackup = 27,
        BackupCollection_BackupCollection = 28,
        Backup_Backup = 29,
        BackupSourceFile_PrepareOnDisc = 30,
        BackupSourceFile_ReadBackupFileIdFromDisc = 31,
        BackupSourceFile_WriteToDisc = 32
    }

    public abstract class Log : GlobalExtention
    {
        public Log(Configuration configuration) : base(configuration)
        {
        }

        public abstract void WriteLog(LogEntry logEntry);
    }
}
