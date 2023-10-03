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
            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                ServerManagerGuiMain serverManagerGuiMain = new ServerManagerGuiMain();
            }
            catch (Exception ex)
            {

            }
        }
    }
}