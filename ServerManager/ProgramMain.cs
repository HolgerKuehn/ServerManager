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
            ThreadCollection threadCollection = new ThreadCollection();

            Log log = new Log();

            try
            {
                log.WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ProgramMain_Main, "starting ServerManager"));

                log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "create new ThreadDynDns"));
                threadCollection.ThreadDynDns(log);


                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "initializing ApplicationConfiguration"));
                ApplicationConfiguration.Initialize();

                log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ProgramMain_Main, "creating new GuiMain"));
                GuiMain GuiMain = new GuiMain(log);
            }
            catch (Exception ex)
            {
                log.WriteLog(new LogEntry(LogSeverity.Critical, LogOrigin.ProgramMain_Main, "caught exception: " + ex.Message.Replace("\"", "\"\"")));
            }
        }
    }
}