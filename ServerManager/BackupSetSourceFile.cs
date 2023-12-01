namespace blog.dachs.ServerManager
{
    using System.Data;

    public enum BackupSetSourceFileState : byte
    {
        toBeBackedUp = 1,
        includedInSet = 2,
        validated = 3
    }

    public class BackupSetSourceFile : GlobalExtention
    {
        private int setSourceFileId;
        private BackupSet set;
        private BackupSourceFile sourceFile;
        private BackupSetSourceFileState state;

        public BackupSetSourceFile(Configuration configuration, BackupSet set, BackupSourceFile sourceFile) : base(configuration)
        {
            this.setSourceFileId = 0;
            this.Set = set;
            this.SourceFile = sourceFile;
            this.state = BackupSetSourceFileState.toBeBackedUp;
        }

        public int SetSourceFileId
        {
            get { return this.setSourceFileId; }
            set { this.setSourceFileId = value; }
        }

        public BackupSet Set
        {
            get { return this.set; }
            set
            {
                if (this.Set != null && !this.Set.Equals(value))
                {
                    this.Changed = true;
                }

                this.set = value;
            }
        }

        public BackupSourceFile SourceFile
        {
            get { return this.sourceFile; }
            set
            {
                if (this.SourceFile != null && !this.SourceFile.Equals(value))
                {
                    this.Changed = true;
                }

                this.sourceFile = value;
            }
        }

        public BackupSetSourceFileState State
        {
            get { return this.state; }
            set
            {
                if (!this.State.Equals(value))
                {
                    this.Changed = true;
                }

                this.state = value;
            }
        }

        public void CreateBackup()
        {
            this.PrepareOnDisc();
            this.ReadFromDisc();
            this.WriteToDisc();
        }

        private void PrepareOnDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSetSourceFile_PrepareOnDisc, "create SetSourceFileId for " + this.SourceFile.FullAbsolutePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSetSourceFile_PrepareOnDisc);
            sqlCommand = sqlCommand.Replace("<BackupSetID>", this.Set.SetId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileID>", this.SourceFile.BackupSourceFileId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSetSourceFile_PrepareOnDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        private void ReadFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSetSourceFile_ReadFromDisc, "read Id for SetSourceFile " + this.SourceFile.FullAbsolutePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSetSourceFile_ReadFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupSetID>", this.Set.SetId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileID>", this.SourceFile.BackupSourceFileId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSetSourceFile_ReadFromDisc, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.SetSourceFileId = Convert.ToInt32(dataRow[0].ToString());
            }
        }

        public void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSetSourceFile_WriteToDisc, "write properties for SetSourceFile " + this.SourceFile.FullAbsolutePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSetSourceFile_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSetSourceFileID>", this.SetSourceFileId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetSourceFileStateID>", Convert.ToString((byte)this.State));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSetSourceFile_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
