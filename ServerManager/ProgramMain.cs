namespace blog.dachs.ServerManager
{
    internal static class ProgramMain
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DataLog dataLog = new DataLog();

            try
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Informational, DataLogOrigin.ProgramMain_Main, "starting ServerManager"));

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ProgramMain_Main, "initializing ApplicationConfiguration"));
                ApplicationConfiguration.Initialize();

                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ProgramMain_Main, "creating new ServerManagerGuiMain"));
                ServerManagerGuiMain serverManagerGuiMain = new ServerManagerGuiMain(dataLog);
            }
            catch (Exception ex)
            {

            }
        }
    }
}