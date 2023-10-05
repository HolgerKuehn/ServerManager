namespace blog.dachs.ServerManager
{
    public partial class GuiWindowLog : Form
    {
        private readonly DataLog dataLog;
        private readonly HandlerSqlite handlerSqlite;

        public GuiWindowLog(DataLog dataLog)
        {
            this.handlerSqlite = new HandlerSqlite();
            this.dataLog = dataLog;

            // initialze Log Window and default values
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_GuiWindowLog, "initializing GuiWindowLog"));
            InitializeComponent();

            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_GuiWindowLog, "initializing clbGuiWindowLogSeverity"));
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Emergency", DataLogSeverity.Emergency), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Alert", DataLogSeverity.Alert), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Critical", DataLogSeverity.Critical), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Error", DataLogSeverity.Error), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Warning", DataLogSeverity.Warning), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Notice", DataLogSeverity.Notice), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Informational", DataLogSeverity.Informational), CheckState.Unchecked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Debug", DataLogSeverity.Debug), CheckState.Unchecked);
        }

        public DataLog DataLog
        {
            get { return this.dataLog; }
        }

        public HandlerSqlite HandlerSqlite
        {
            get { return this.handlerSqlite; }
        }

        private void GuiWindowLog_Shown(object sender, EventArgs e)
        {
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_GuiWindowLog_Shown, "initializing clbGuiWindowLogSeverity"));
            this.Hide();
        }


        private void tmrGuiWindowLog_Tick(object sender, EventArgs e)
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


            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_tmrGuiWindowLog_Tick, "reading DataTable dgvGuiWindowLogLog"));
            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_tmrGuiWindowLog_Tick, command));
            dgvGuiWindowLogLog.DataSource = this.handlerSqlite.GetDataTable(command);

            dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_tmrGuiWindowLog_Tick, "setting column properties on dgvGuiWindowLogLog"));
            
            // DataLog_ID
            dgvGuiWindowLogLog.Columns[0].Visible = false;

            // DataLog_Timestamp
            dgvGuiWindowLogLog.Columns[1].Visible = true;
            dgvGuiWindowLogLog.Columns[1].HeaderText = "Time";
            dgvGuiWindowLogLog.Columns[1].MinimumWidth = 150;

            // DataLogSeverity_Name
            dgvGuiWindowLogLog.Columns[2].Visible = true;
            dgvGuiWindowLogLog.Columns[2].HeaderText = "Severity";
            dgvGuiWindowLogLog.Columns[2].MinimumWidth = 150;

            // DataLogOrigin_Class
            dgvGuiWindowLogLog.Columns[3].Visible = true;
            dgvGuiWindowLogLog.Columns[3].HeaderText = "Class";
            dgvGuiWindowLogLog.Columns[3].MinimumWidth = 150;

            // DataLogOrigin_Function
            dgvGuiWindowLogLog.Columns[4].Visible = true;
            dgvGuiWindowLogLog.Columns[4].HeaderText = "Function";
            dgvGuiWindowLogLog.Columns[4].MinimumWidth = 150;

            // DataLogOrigin_Step
            dgvGuiWindowLogLog.Columns[5].Visible = true;
            dgvGuiWindowLogLog.Columns[5].HeaderText = "Step";
            dgvGuiWindowLogLog.Columns[5].MinimumWidth = 150;

            // DataLog_Message
            dgvGuiWindowLogLog.Columns[6].Visible = true;
            dgvGuiWindowLogLog.Columns[6].HeaderText = "Message";
            dgvGuiWindowLogLog.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void GuiWindowLog_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "showing GuiWindowLog"));
                tmrGuiWindowLog.Enabled = true;
                tmrGuiWindowLog_Tick(sender, e);
            }
            else
            {
                dataLog.WriteLog(new DataLogEntry(DataLogSeverity.Debug, DataLogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "hiding GuiWindowLog"));
                tmrGuiWindowLog.Enabled = false;
            }
        }
    }
}