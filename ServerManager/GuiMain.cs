namespace blog.dachs.ServerManager
{
    internal class GuiMain : GlobalExtention
    {
        private readonly NotifyIcon systemTrayIcon;
        private readonly Form windowLog;

        public GuiMain(Configuration configuration) : base(configuration)
        {
            // initialize system tray icon
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "initializing systemTrayIcon"));
            systemTrayIcon = new NotifyIcon();
            systemTrayIcon.Icon = new Icon("Icon\\48.ico");
            systemTrayIcon.Visible = true;

            systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
            systemTrayIcon.ContextMenuStrip.Items.Add("Log", Image.FromFile("Icon\\48.ico"), SystemTrayIcon_OnLogClicked);

            // initialize log window
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "initializing GuiWindowLog"));
            windowLog = new GuiWindowLog(configuration);
            Application.Run(windowLog);
        }

        private void SystemTrayIcon_OnLogClicked(object? sender, EventArgs e)
        {
            if (windowLog.Visible)
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "hiding GuiWindowLog"));
                windowLog.Hide();
            }
            else
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiMain_GuiMain, "showing GuiWindowLog"));
                windowLog.Show();
            }
        }

        public NotifyIcon SystemTrayIcon
        {
            get { return systemTrayIcon; }
        }
    }
}
