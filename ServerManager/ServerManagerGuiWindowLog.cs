namespace blog.dachs.ServerManager
{
    public partial class ServerManagerGuiWindowLog : Form
    {
        private HandlerSqlite handlerSqlite;

        public ServerManagerGuiWindowLog(HandlerSqlite serverManagerSqlite)
        {
            this.handlerSqlite = serverManagerSqlite;

            // initialze Log Window and default values
            InitializeComponent();

            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Emergency", DataLogSeverity.Emergency), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Alert", DataLogSeverity.Alert), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Critical", DataLogSeverity.Critical), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Error", DataLogSeverity.Error), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Warning", DataLogSeverity.Warning), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Notice", DataLogSeverity.Notice), CheckState.Checked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Informational", DataLogSeverity.Informational), CheckState.Unchecked);
            clbServerManagerGuiWindowLogSeverity.Items.Add(new ServerManagerGuiCheckedListBoxItem("Debug", DataLogSeverity.Debug), CheckState.Unchecked);
        }

        private void WindowLog_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }


        private void tmrServerManagerGuiWindowLog_Tick(object sender, EventArgs e)
        {
            string command = string.Empty;

            command += "select a.DataLog_ID, datetime(a.DataLog_Timestamp, 'unixepoch', 'localtime') as DataLog_Timestamp, b.DataLogSeverity_Name, c.DataLogOrigin_Class, c.DataLogOrigin_Function, c.DataLogOrigin_Step, a.DataLog_Message ";
            command += "from DataLog as a ";
            command += "inner join DataLogSeverity as b on ";
            command += "    a.DataLogSeverity_ID = b.DataLogSeverity_ID ";
            command += "inner join DataLogOrigin as c on ";
            command += "    a.DataLogOrigin_ID = c.DataLogOrigin_ID ";
            command += "order by a.DataLog_ID desc ";
            command += "limit 10000; ";


            dgvServerManagerGuiWindowLogLog.DataSource = this.handlerSqlite.GetDataTable(command);

            // DataLog_ID
            dgvServerManagerGuiWindowLogLog.Columns[0].Visible = false;

            // DataLog_Timestamp
            dgvServerManagerGuiWindowLogLog.Columns[1].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[1].HeaderText = "Time";
            dgvServerManagerGuiWindowLogLog.Columns[1].MinimumWidth = 150;

            // DataLogSeverity_Name
            dgvServerManagerGuiWindowLogLog.Columns[2].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[2].HeaderText = "Severity";
            dgvServerManagerGuiWindowLogLog.Columns[2].MinimumWidth = 150;

            // DataLogOrigin_Class
            dgvServerManagerGuiWindowLogLog.Columns[3].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[3].HeaderText = "Class";
            dgvServerManagerGuiWindowLogLog.Columns[3].MinimumWidth = 150;

            // DataLogOrigin_Function
            dgvServerManagerGuiWindowLogLog.Columns[4].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[4].HeaderText = "Function";
            dgvServerManagerGuiWindowLogLog.Columns[4].MinimumWidth = 150;

            // DataLogOrigin_Step
            dgvServerManagerGuiWindowLogLog.Columns[5].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[5].HeaderText = "Step";
            dgvServerManagerGuiWindowLogLog.Columns[5].MinimumWidth = 150;

            // DataLog_Message
            dgvServerManagerGuiWindowLogLog.Columns[6].Visible = true;
            dgvServerManagerGuiWindowLogLog.Columns[6].HeaderText = "Message";
            dgvServerManagerGuiWindowLogLog.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void ServerManagerGuiWindowLog_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                tmrServerManagerGuiWindowLog.Enabled = true;
                tmrServerManagerGuiWindowLog_Tick(sender, e);
            }
            else
            {
                tmrServerManagerGuiWindowLog.Enabled = false;
            }
        }
    }
}