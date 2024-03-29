﻿namespace blog.dachs.ServerManager
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
        DynDnsIpAddress_PrepareIpAddressToDisc = 14,
        DynDnsService_ReadIpAddressCollectionFromDisc = 22,
        DynDnsService_UpdatePublicDnsIpAddress = 18,
        DynDnsIpAddress_WriteIpAddress = 15,
        DynDnsService_WriteLogForChangedIpAddress = 20,
        DynDnsServiceLocal_UpdatePublicDnsIpAddress = 19,
        DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromPowerShell = 23,
        DynDnsFirewallRule_DynDnsFirewallRule = 24,
        DynDnsFirewallRule_ReadFirewallRuleBaseProperties = 25,
        DynDnsIpCollection_ReadIpAddressCollection = 26,
        ThreadBackup_ThreadBackup = 27,
        BackupCollection_BackupCollection = 28,
        Backup_Backup = 29,
        BackupSourceFile_PrepareOnDisc = 30,
        BackupSourceFile_ReadFromDisc = 31,
        BackupSourceFile_WriteToDisc = 32,
        BackupSource_PrepareOnDisc = 33,
        BackupSource_ReadFromDiscByPath = 34,
        BackupSource_WriteToDisc = 35,
        BackupSet_PrepareOnDisc = 36,
        BackupSet_ReadFromDisc = 37,
        BackupSet_WriteToDisc = 38,
        BackupSetSourceFile_PrepareOnDisc = 39,
        BackupSetSourceFile_ReadFromDisc = 40,
        BackupSetSourceFile_WriteToDisc = 41,
        KeePassDatabase_KeePassDatabase = 42,
        KeePassDatabase_WriteToDisc = 43,
        KeePassDatabaseCollection_KeePassDatabaseCollection = 44,
        ServiceWorker_Task = 45,
        BackupSourceCollection_CreateBackup_ReadFileList = 46,
        BackupSourceFileCollection_ReadFileList = 47,
        BackupDestinationDevice_BackupDestinationDevice = 48,
        BackupDestinationDevice_WriteToDisc = 49,
        BackupSource_CreateBackupSet = 50,
        BackupSetCollection_BackupSetCollection = 51,
        BackupSourceFileCollection_CreateBackup = 52,
        BackupSet_CreateBackup_WriteFileList = 53,
        BackupSet_CreateBackup_ReadFileList = 54,
        BackupSet_CreateBackup_ValidateBackup = 55,
        BackupSet_CreateBackup_LastBackupTimestamp = 56,
        BackupSet_CreateBackup_ActualSize = 57,
        BackupSet_CreateBackup_ProjectedSize = 58,
        BackupSet_CreateBackup_CompareSize = 59,
        DynDnsIpAddress_DynDnsIpAddress = 60,
        DynDnsFirewallRule_PrepareFirewallRuleToDisc = 61,
        DynDnsFirewallRule_WriteFirewallRuleToDisc = 62,
        DynDnsFirewallRuleCollection_ReadFirewallRuleCollectionFromDisc = 63,
        ThreadFirewallRules_Work = 64,
        DynDnsFirewallRule_WriteRemoteAddress = 65
    }

    public abstract class Log : GlobalExtention
    {
        public Log(Configuration configuration) : base(configuration)
        {
        }

        public abstract void WriteLog(LogEntry logEntry);
    }
}
