namespace blog.dachs.ServerManager
{
    internal class GuiMain
    {
        private readonly Log Log;
        private readonly NotifyIcon systemTrayIcon;
        private readonly Form windowLog;

        public GuiMain(Log Log)
        {
            this.Log = Log;

            // initialize system tray icon
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "initializing systemTrayIcon"));
            systemTrayIcon = new NotifyIcon();
            systemTrayIcon.Icon = new Icon("Icon\\48.ico");
            systemTrayIcon.Visible = true;

            systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
            systemTrayIcon.ContextMenuStrip.Items.Add("Log", Image.FromFile("Icon\\48.ico"), SystemTrayIcon_OnLogClicked);

            // initialize log window
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "initializing GuiWindowLog"));
            windowLog = new GuiWindowLog(Log);
            Application.Run(windowLog);
        }

        private void SystemTrayIcon_OnLogClicked(object? sender, EventArgs e)
        {
            if (windowLog.Visible)
            {
                Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "hiding GuiWindowLog"));
                windowLog.Hide();
            }
            else
            {
                Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "showing GuiWindowLog"));
                windowLog.Show();
            }
        }

        public NotifyIcon SystemTrayIcon
        {
            get { return systemTrayIcon; }
        }
    }
}
