namespace blog.dachs.ServerManager
{
    internal class GuiMain
    {
        private readonly NotifyIcon systemTrayIcon;
        private readonly Form windowLog;
        private readonly DataLog dataLog;

        public GuiMain(DataLog dataLog)
        {
            this.dataLog = dataLog;

            // initialize system tray icon
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiMain_GuiMain, "initializing systemTrayIcon"));
            systemTrayIcon = new NotifyIcon();
            systemTrayIcon.Icon = new Icon("Icon\\48.ico");
            systemTrayIcon.Visible = true;

            systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
            systemTrayIcon.ContextMenuStrip.Items.Add("Log", Image.FromFile("Icon\\48.ico"), SystemTrayIcon_OnLogClicked);

            // initialize log window
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiMain_GuiMain, "initializing GuiWindowLog"));
            windowLog = new GuiWindowLog(dataLog);
            Application.Run(windowLog);
        }

        private void SystemTrayIcon_OnLogClicked(object? sender, EventArgs e)
        {
            if (windowLog.Visible)
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiMain_GuiMain, "hiding GuiWindowLog"));
                windowLog.Hide();
            }
            else
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiMain_GuiMain, "showing GuiWindowLog"));
                windowLog.Show();
            }
        }

        public NotifyIcon SystemTrayIcon
        {
            get { return systemTrayIcon; }
        }
    }
}
