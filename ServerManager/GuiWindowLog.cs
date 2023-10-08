namespace blog.dachs.ServerManager
{
    public partial class GuiWindowLog : Form
    {
        private readonly Log log;
        private readonly HandlerDatabase handlerDatabase;

        public GuiWindowLog(Log log)
        {
            this.handlerDatabase = HandlerDatabase.GetHandlerDatabase();
            this.log = log;

            // initialze Log Window and default values
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog, "initializing GuiWindowLog"));
            InitializeComponent();

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog, "initializing clbGuiWindowLogSeverity"));
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Emergency", LogSeverity.Emergency), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Alert", LogSeverity.Alert), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Critical", LogSeverity.Critical), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Error", LogSeverity.Error), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Warning", LogSeverity.Warning), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Notice", LogSeverity.Notice), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Informational", LogSeverity.Informational), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Debug", LogSeverity.Debug), CheckState.Unchecked);
        }

        public Log Log
        {
            get { return this.log; }
        }

        public HandlerDatabase HandlerDatabase
        {
            get { return this.handlerDatabase; }
        }

        private void GuiWindowLog_Shown(object sender, EventArgs e)
        {
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_Shown, "initializing clbGuiWindowLogSeverity"));
            this.Hide();
        }


        private void TmrGuiWindowLog_Tick(object sender, EventArgs e)
        {
            string command = string.Empty;

            command += "select a.Log_ID, datetime(a.Log_Timestamp, 'unixepoch', 'localtime') as Log_Timestamp, b.LogSeverity_Name, c.LogOrigin_Class, c.LogOrigin_Function, c.LogOrigin_Step, a.Log_Message ";
            command += "from Log as a ";
            command += "inner join LogSeverity as b on ";
            command += "    a.LogSeverity_ID = b.LogSeverity_ID ";
            command += "inner join LogOrigin as c on ";
            command += "    a.LogOrigin_ID = c.LogOrigin_ID ";
            command += "order by a.Log_ID desc ";
            command += "limit 10000; ";


            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, "reading DataTable dgvGuiWindowLogLog"));
            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, command));
            dgvGuiWindowLogLog.DataSource = this.HandlerDatabase.GetDataTable(command);

            Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, "setting column properties on dgvGuiWindowLogLog"));
            
            // Log_ID
            dgvGuiWindowLogLog.Columns[0].Visible = false;

            // Log_Timestamp
            dgvGuiWindowLogLog.Columns[1].Visible = true;
            dgvGuiWindowLogLog.Columns[1].HeaderText = "Time";
            dgvGuiWindowLogLog.Columns[1].MinimumWidth = 150;

            // LogSeverity_Name
            dgvGuiWindowLogLog.Columns[2].Visible = true;
            dgvGuiWindowLogLog.Columns[2].HeaderText = "Severity";
            dgvGuiWindowLogLog.Columns[2].MinimumWidth = 150;

            // LogOrigin_Class
            dgvGuiWindowLogLog.Columns[3].Visible = true;
            dgvGuiWindowLogLog.Columns[3].HeaderText = "Class";
            dgvGuiWindowLogLog.Columns[3].MinimumWidth = 150;

            // LogOrigin_Function
            dgvGuiWindowLogLog.Columns[4].Visible = true;
            dgvGuiWindowLogLog.Columns[4].HeaderText = "Function";
            dgvGuiWindowLogLog.Columns[4].MinimumWidth = 150;

            // LogOrigin_Step
            dgvGuiWindowLogLog.Columns[5].Visible = true;
            dgvGuiWindowLogLog.Columns[5].HeaderText = "Step";
            dgvGuiWindowLogLog.Columns[5].MinimumWidth = 150;

            // Log_Message
            dgvGuiWindowLogLog.Columns[6].Visible = true;
            dgvGuiWindowLogLog.Columns[6].HeaderText = "Message";
            dgvGuiWindowLogLog.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void GuiWindowLog_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "showing GuiWindowLog"));
                tmrGuiWindowLog.Enabled = true;
                TmrGuiWindowLog_Tick(sender, e);
            }
            else
            {
                Log.WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "hiding GuiWindowLog"));
                tmrGuiWindowLog.Enabled = false;
            }
        }
    }
}