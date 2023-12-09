namespace blog.dachs.ServerManager
{
    using System.Data;
    
    public class BackupSource : GlobalExtention
    {
        private int sourceId;
        private Backup backup;
        private string path;
        private string password;
        private BackupSourceFileCollection backupSourceFileCollection;
        private BackupSetCollection backupSetCollection;

        public BackupSource(Configuration configuration, Backup backup) : base(configuration)
        {
            this.SourceId = 0;
            this.Backup = backup;
            this.Path = string.Empty;
            this.Password= string.Empty;
            this.SourceFileCollection = new BackupSourceFileCollection(configuration, this);
            this.SetCollection = new BackupSetCollection(configuration, this);
        }

        public int SourceId
        {
            get { return this.sourceId; }
            set { this.sourceId = value; }
        }

        public Backup Backup
        {
            get { return this.backup; }
            set
            {
                if (this.Backup != null && !this.Backup.Equals(value))
                {
                    this.Changed = true;
                }

                this.backup = value;
            }
        }

        public string Path
        {
            get { return this.path; }
            set
            {
                if (this.Path != null && !this.Path.Equals(value))
                {
                    this.Changed = true;
                }

                this.path = value;
            }
        }

        public string FullAbsolutePath
        {
            get 
            {
                string path;
                
                path = this.Backup.SourceBasePath;
                
                if (this.Path != string.Empty)
                {
                    path += "\\" + this.Path;
                }

                return path;
            }
            set { }
        }

        public string FullRelativePath
        {
            get { return this.Path; }
            set { }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.Password != null && !this.Password.Equals(value))
                {
                    this.Changed = true;
                }

                this.password = value;
            }
        }

        public BackupSourceFileCollection SourceFileCollection
        {
            get { return this.backupSourceFileCollection; }
            set { this.backupSourceFileCollection = value; }
        }

        public BackupSetCollection SetCollection
        {
            get { return this.backupSetCollection; }
            set { this.backupSetCollection = value; }
        }

        public void CreateBackup()
        {
            this.PrepareOnDisc();
            this.ReadFromDisc();
            this.WriteToDisc();

            this.SourceFileCollection.CreateBackup();

            if(this.SourceFileCollection.GetFilesToBackup().Count > 0)
            {
                KeePassEntry keePassEntry = this.Backup.KeePassDatabase.GetEntry(this.Path);
                this.Password = keePassEntry.Password;

                DateTime date = DateTime.UtcNow;
                BackupSet backupSet = new BackupSet(this.Configuration, this);

                backupSet.Path = this.Path;
                backupSet.Name = date.Year.ToString().PadLeft(4, '0') + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0') + date.Hour.ToString().PadLeft(2, '0') + date.Minute.ToString().PadLeft(2, '0');

                backupSet.CreateBackup();
                
                this.SetCollection.Add(backupSet);
            }
        }

        private void PrepareOnDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSource_PrepareOnDisc, "create SourceId for " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSource_PrepareOnDisc);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.Backup.BackupId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourcePath>", this.Path);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSource_PrepareOnDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        private void ReadFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSource_ReadFromDisc, "read Id for Source " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSource_ReadFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.Backup.BackupId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourcePath>", this.Path);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSource_ReadFromDisc, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.SourceId = Convert.ToInt32(dataRow[0].ToString());
            }
        }

        private void WriteToDisc()
        {
        }
    }
}
