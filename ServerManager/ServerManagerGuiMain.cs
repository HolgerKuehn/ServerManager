using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager
{
    internal class ServerManagerGuiMain
    {
        private readonly NotifyIcon systemTrayIcon;
        private readonly Form windowLog;

        public ServerManagerGuiMain()
        {
            // initialize system tray icon
            systemTrayIcon = new NotifyIcon();
            systemTrayIcon.Icon = new Icon("Icon\\48.ico");
            systemTrayIcon.Visible = true;

            systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
            systemTrayIcon.ContextMenuStrip.Items.Add("Log", Image.FromFile("Icon\\48.ico"), SystemTrayIcon_OnLogClicked);

            // initialize log window
            windowLog = new ServerManagerGuiWindowLog(new ServerManagerSqlite());
            Application.Run(windowLog);
        }

        private void SystemTrayIcon_OnLogClicked(object? sender, EventArgs e)
        {
            if (windowLog.Visible)
            {
                windowLog.Hide();
            }
            else
            {
                windowLog.Show();
            }
        }


        public NotifyIcon SystemTrayIcon => systemTrayIcon;


    }
}
