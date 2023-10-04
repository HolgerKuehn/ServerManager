namespace blog.dachs.ServerManager
{
    internal class ServerManagerGuiMain
    {
        private readonly NotifyIcon systemTrayIcon;
        private readonly Form windowLog;
        private readonly DataLog dataLog;

        public ServerManagerGuiMain(DataLog dataLog)
        {
            this.dataLog = dataLog;

            // initialize system tray icon
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ServerManagerGuiMain_ServerManagerGuiMain, "initializing systemTrayIcon"));
            systemTrayIcon = new NotifyIcon();
            systemTrayIcon.Icon = new Icon("Icon\\48.ico");
            systemTrayIcon.Visible = true;

            systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
            systemTrayIcon.ContextMenuStrip.Items.Add("Log", Image.FromFile("Icon\\48.ico"), SystemTrayIcon_OnLogClicked);

            // initialize log window
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ServerManagerGuiMain_ServerManagerGuiMain, "initializing ServerManagerGuiWindowLog"));
            windowLog = new ServerManagerGuiWindowLog(new HandlerSqlite());
            Application.Run(windowLog);
        }

        private void SystemTrayIcon_OnLogClicked(object? sender, EventArgs e)
        {
            if (windowLog.Visible)
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ServerManagerGuiMain_ServerManagerGuiMain, "hiding ServerManagerGuiWindowLog"));
                windowLog.Hide();
            }
            else
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.ServerManagerGuiMain_ServerManagerGuiMain, "showing ServerManagerGuiWindowLog"));
                windowLog.Show();
            }
        }

        public NotifyIcon SystemTrayIcon
        {
            get { return systemTrayIcon; }
        }
    }
}
