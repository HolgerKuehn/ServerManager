namespace blog.dachs.ServerManager
{
    partial class ServerManagerGuiWindowLog
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private GroupBox grbServerManagerGuiWindowLogFilter;
        private CheckedListBox clbServerManagerGuiWindowLogSeverity;
        private Splitter splServerManagerGuiWindowLog1;
        private CheckedListBox clbServerManagerGuiWindowLogColumns;
        private Splitter splServerManagerGuiWindowLog2;
        private DataGridView dgvServerManagerGuiWindowLogLog;
        private System.Windows.Forms.Timer tmrServerManagerGuiWindowLog;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerManagerGuiWindowLog));
            grbServerManagerGuiWindowLogFilter = new GroupBox();
            clbServerManagerGuiWindowLogColumns = new CheckedListBox();
            splServerManagerGuiWindowLog1 = new Splitter();
            clbServerManagerGuiWindowLogSeverity = new CheckedListBox();
            splServerManagerGuiWindowLog2 = new Splitter();
            dgvServerManagerGuiWindowLogLog = new DataGridView();
            tmrServerManagerGuiWindowLog = new System.Windows.Forms.Timer(components);
            grbServerManagerGuiWindowLogFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvServerManagerGuiWindowLogLog).BeginInit();
            SuspendLayout();
            // 
            // grbServerManagerGuiWindowLogFilter
            // 
            grbServerManagerGuiWindowLogFilter.Controls.Add(clbServerManagerGuiWindowLogColumns);
            grbServerManagerGuiWindowLogFilter.Controls.Add(splServerManagerGuiWindowLog1);
            grbServerManagerGuiWindowLogFilter.Controls.Add(clbServerManagerGuiWindowLogSeverity);
            grbServerManagerGuiWindowLogFilter.Dock = DockStyle.Top;
            grbServerManagerGuiWindowLogFilter.Location = new Point(0, 0);
            grbServerManagerGuiWindowLogFilter.Name = "grbServerManagerGuiWindowLogFilter";
            grbServerManagerGuiWindowLogFilter.Size = new Size(800, 97);
            grbServerManagerGuiWindowLogFilter.TabIndex = 0;
            grbServerManagerGuiWindowLogFilter.TabStop = false;
            grbServerManagerGuiWindowLogFilter.Text = "Filter";
            // 
            // clbServerManagerGuiWindowLogColumns
            // 
            clbServerManagerGuiWindowLogColumns.Dock = DockStyle.Fill;
            clbServerManagerGuiWindowLogColumns.FormattingEnabled = true;
            clbServerManagerGuiWindowLogColumns.Location = new Point(417, 19);
            clbServerManagerGuiWindowLogColumns.Name = "clbServerManagerGuiWindowLogColumns";
            clbServerManagerGuiWindowLogColumns.Size = new Size(380, 75);
            clbServerManagerGuiWindowLogColumns.TabIndex = 3;
            // 
            // splServerManagerGuiWindowLog1
            // 
            splServerManagerGuiWindowLog1.Location = new Point(414, 19);
            splServerManagerGuiWindowLog1.Name = "splServerManagerGuiWindowLog1";
            splServerManagerGuiWindowLog1.Size = new Size(3, 75);
            splServerManagerGuiWindowLog1.TabIndex = 1;
            splServerManagerGuiWindowLog1.TabStop = false;
            // 
            // clbServerManagerGuiWindowLogSeverity
            // 
            clbServerManagerGuiWindowLogSeverity.Dock = DockStyle.Left;
            clbServerManagerGuiWindowLogSeverity.FormattingEnabled = true;
            clbServerManagerGuiWindowLogSeverity.Location = new Point(3, 19);
            clbServerManagerGuiWindowLogSeverity.Name = "clbServerManagerGuiWindowLogSeverity";
            clbServerManagerGuiWindowLogSeverity.ScrollAlwaysVisible = true;
            clbServerManagerGuiWindowLogSeverity.Size = new Size(411, 75);
            clbServerManagerGuiWindowLogSeverity.TabIndex = 0;
            // 
            // splServerManagerGuiWindowLog2
            // 
            splServerManagerGuiWindowLog2.Dock = DockStyle.Top;
            splServerManagerGuiWindowLog2.Location = new Point(0, 97);
            splServerManagerGuiWindowLog2.Name = "splServerManagerGuiWindowLog2";
            splServerManagerGuiWindowLog2.Size = new Size(800, 3);
            splServerManagerGuiWindowLog2.TabIndex = 1;
            splServerManagerGuiWindowLog2.TabStop = false;
            // 
            // dgvServerManagerGuiWindowLogLog
            // 
            dgvServerManagerGuiWindowLogLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvServerManagerGuiWindowLogLog.Dock = DockStyle.Fill;
            dgvServerManagerGuiWindowLogLog.Location = new Point(0, 100);
            dgvServerManagerGuiWindowLogLog.Name = "dgvServerManagerGuiWindowLogLog";
            dgvServerManagerGuiWindowLogLog.RowTemplate.Height = 25;
            dgvServerManagerGuiWindowLogLog.Size = new Size(800, 350);
            dgvServerManagerGuiWindowLogLog.TabIndex = 2;
            // 
            // tmrServerManagerGuiWindowLog
            // 
            tmrServerManagerGuiWindowLog.Enabled = true;
            tmrServerManagerGuiWindowLog.Interval = 60000;
            tmrServerManagerGuiWindowLog.Tick += tmrServerManagerGuiWindowLog_Tick;
            // 
            // ServerManagerGuiWindowLog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvServerManagerGuiWindowLogLog);
            Controls.Add(splServerManagerGuiWindowLog2);
            Controls.Add(grbServerManagerGuiWindowLogFilter);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ServerManagerGuiWindowLog";
            Text = "Server Manager";
            Shown += WindowLog_Shown;
            VisibleChanged += ServerManagerGuiWindowLog_VisibleChanged;
            grbServerManagerGuiWindowLogFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvServerManagerGuiWindowLogLog).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}