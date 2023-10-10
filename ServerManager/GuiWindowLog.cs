using System.Data;

namespace blog.dachs.ServerManager
{
    public partial class GuiWindowLog : GuiExtention
    {
        public GuiWindowLog(Configuration configuration) : base(configuration)
        {
            // initialze GetLog Window and default values
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog, "initializing GuiWindowLog"));
            InitializeComponent();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog, "initializing clbGuiWindowLogSeverity"));
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Emergency", LogSeverity.Emergency), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Alert", LogSeverity.Alert), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Critical", LogSeverity.Critical), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Error", LogSeverity.Error), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Warning", LogSeverity.Warning), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Notice", LogSeverity.Notice), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Informational", LogSeverity.Informational), CheckState.Checked);
            clbGuiWindowLogSeverity.Items.Add(new GuiCheckedListBoxItem("Debug", LogSeverity.Debug), CheckState.Unchecked);
        }


        private void GuiWindowLog_Shown(object sender, EventArgs e)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_Shown, "initializing clbGuiWindowLogSeverity"));
            this.Hide();
        }


        private void TmrGuiWindowLog_Tick(object sender, EventArgs e)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, "reading DataTable dgvGuiWindowLogLog"));
            
            string sqlCommand = this.HandlerDatabase.GetCommand(Command.GuiWindowLog_TmrGuiWindowLog_Tick);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, sqlCommand));

            dgvGuiWindowLogLog.DataSource = this.HandlerDatabase.GetDataTable(sqlCommand);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_TmrGuiWindowLog_Tick, "setting column properties on dgvGuiWindowLogLog"));
            
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
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "showing GuiWindowLog"));
                tmrGuiWindowLog.Enabled = true;
                TmrGuiWindowLog_Tick(sender, e);
            }
            else
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.GuiWindowLog_GuiWindowLog_VisibleChanged, "hiding GuiWindowLog"));
                tmrGuiWindowLog.Enabled = false;
            }
        }
    }
}