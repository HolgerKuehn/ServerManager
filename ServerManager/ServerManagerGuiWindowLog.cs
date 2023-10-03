namespace blog.dachs.ServerManager
{
    public partial class ServerManagerGuiWindowLog : Form
    {
        private ServerManagerSqlite serverManagerSqlite;

        public ServerManagerGuiWindowLog(ServerManagerSqlite serverManagerSqlite)
        {
            this.serverManagerSqlite = serverManagerSqlite;

            // initialze main window and system tray icon
            InitializeComponent();
        }

        private void WindowLog_Load(object sender, EventArgs e)
        {
            
            dataGridView1.DataSource = this.serverManagerSqlite.GetDataTable("select a.ServerManagerConfiguration_ID, a.ServerManagerConfiguration_ServerName, a.ServerManagerConfiguration_DynDNS from ServerManagerConfiguration a");
            //dataGridView1.Columns[0].Visible = false;
            //dataGridView1.Columns[1].Visible = false;
            //dataGridView1.Columns[2].Visible = false;
            //dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void WindowLog_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}