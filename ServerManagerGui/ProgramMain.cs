namespace blog.dachs.ServerManager
{
    public static class ProgramMain
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Configuration configuration = new Configuration();

            try
            {
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ProgramMain_Main, "starting ServerManager on \"" + Environment.MachineName + "\""));

                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "create new ThreadDynDns"));
                configuration.ThreadCollection.ThreadDynDns(configuration);

                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "create new ThreadBackup"));
                configuration.ThreadCollection.ThreadBackup(configuration);

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "initializing ApplicationConfiguration"));
                ApplicationConfiguration.Initialize();

                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "creating new GuiMain"));
                GuiMain GuiMain = new GuiMain(configuration);
            }
            catch (Exception ex)
            {
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Critical, LogOrigin.ProgramMain_Main, "caught exception: " + ex.Message.Replace("\"", "\"\"")));
            }
        }
    }
}