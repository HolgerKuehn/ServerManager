namespace blog.dachs.ServerManager
{
    partial class GuiWindowLog
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private GroupBox grbGuiWindowLogFilter;
        private CheckedListBox clbGuiWindowLogSeverity;
        private Splitter splGuiWindowLog1;
        private CheckedListBox clbGuiWindowLogColumns;
        private Splitter splGuiWindowLog2;
        private DataGridView dgvGuiWindowLogLog;
        private System.Windows.Forms.Timer tmrGuiWindowLog;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuiWindowLog));
            grbGuiWindowLogFilter = new GroupBox();
            clbGuiWindowLogColumns = new CheckedListBox();
            splGuiWindowLog1 = new Splitter();
            clbGuiWindowLogSeverity = new CheckedListBox();
            splGuiWindowLog2 = new Splitter();
            dgvGuiWindowLogLog = new DataGridView();
            tmrGuiWindowLog = new System.Windows.Forms.Timer(components);
            grbGuiWindowLogFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGuiWindowLogLog).BeginInit();
            SuspendLayout();
            // 
            // grbGuiWindowLogFilter
            // 
            grbGuiWindowLogFilter.Controls.Add(clbGuiWindowLogColumns);
            grbGuiWindowLogFilter.Controls.Add(splGuiWindowLog1);
            grbGuiWindowLogFilter.Controls.Add(clbGuiWindowLogSeverity);
            grbGuiWindowLogFilter.Dock = DockStyle.Top;
            grbGuiWindowLogFilter.Location = new Point(0, 0);
            grbGuiWindowLogFilter.Name = "grbGuiWindowLogFilter";
            grbGuiWindowLogFilter.Size = new Size(800, 97);
            grbGuiWindowLogFilter.TabIndex = 0;
            grbGuiWindowLogFilter.TabStop = false;
            grbGuiWindowLogFilter.Text = "Filter";
            // 
            // clbGuiWindowLogColumns
            // 
            clbGuiWindowLogColumns.Dock = DockStyle.Fill;
            clbGuiWindowLogColumns.FormattingEnabled = true;
            clbGuiWindowLogColumns.Location = new Point(417, 19);
            clbGuiWindowLogColumns.Name = "clbGuiWindowLogColumns";
            clbGuiWindowLogColumns.Size = new Size(380, 75);
            clbGuiWindowLogColumns.TabIndex = 3;
            // 
            // splGuiWindowLog1
            // 
            splGuiWindowLog1.Location = new Point(414, 19);
            splGuiWindowLog1.Name = "splGuiWindowLog1";
            splGuiWindowLog1.Size = new Size(3, 75);
            splGuiWindowLog1.TabIndex = 1;
            splGuiWindowLog1.TabStop = false;
            // 
            // clbGuiWindowLogSeverity
            // 
            clbGuiWindowLogSeverity.Dock = DockStyle.Left;
            clbGuiWindowLogSeverity.FormattingEnabled = true;
            clbGuiWindowLogSeverity.Location = new Point(3, 19);
            clbGuiWindowLogSeverity.Name = "clbGuiWindowLogSeverity";
            clbGuiWindowLogSeverity.ScrollAlwaysVisible = true;
            clbGuiWindowLogSeverity.Size = new Size(411, 75);
            clbGuiWindowLogSeverity.TabIndex = 0;
            // 
            // splGuiWindowLog2
            // 
            splGuiWindowLog2.Dock = DockStyle.Top;
            splGuiWindowLog2.Location = new Point(0, 97);
            splGuiWindowLog2.Name = "splGuiWindowLog2";
            splGuiWindowLog2.Size = new Size(800, 3);
            splGuiWindowLog2.TabIndex = 1;
            splGuiWindowLog2.TabStop = false;
            // 
            // dgvGuiWindowLogLog
            // 
            dgvGuiWindowLogLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGuiWindowLogLog.Dock = DockStyle.Fill;
            dgvGuiWindowLogLog.Location = new Point(0, 100);
            dgvGuiWindowLogLog.Name = "dgvGuiWindowLogLog";
            dgvGuiWindowLogLog.RowTemplate.Height = 25;
            dgvGuiWindowLogLog.Size = new Size(800, 350);
            dgvGuiWindowLogLog.TabIndex = 2;
            // 
            // tmrGuiWindowLog
            // 
            tmrGuiWindowLog.Enabled = true;
            tmrGuiWindowLog.Interval = 60000;
            tmrGuiWindowLog.Tick += TmrGuiWindowLog_Tick;
            // 
            // GuiWindowLog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvGuiWindowLogLog);
            Controls.Add(splGuiWindowLog2);
            Controls.Add(grbGuiWindowLogFilter);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "GuiWindowLog";
            Text = "Server Manager";
            Shown += GuiWindowLog_Shown;
            VisibleChanged += GuiWindowLog_VisibleChanged;
            grbGuiWindowLogFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGuiWindowLogLog).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}