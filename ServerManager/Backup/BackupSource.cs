namespace blog.dachs.ServerManager.Backup
{
    using System.Data;
    
    public class BackupSource : GlobalExtention
    {
        private int sourceId;
        private Backup backup;
        private string path;
        private string password;
        private DateTime lastSeenDate;

        public BackupSource(Configuration configuration, Backup backup) : base(configuration)
        {
            this.SourceId = 0;
            this.Backup = backup;
            this.Path = string.Empty;
            this.Password= string.Empty;
        }

        //public BackupSource(Configuration configuration, int sourceId) : base(configuration)
        //{
        //    this.SourceId = sourceId;
        //    this.Backup = backup;
        //    this.Path = string.Empty;
        //    this.Password = string.Empty;
        //}

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
            get
            {
                string path = this.path;

                if (path == string.Empty || path is null)
                {
                    path = string.Empty;
                }

                return path;
            }
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
                
                path = this.Backup.SourceBasePathWithDevice;
                
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

        public DateTime LastSeenDate
        {
            get { return this.lastSeenDate; }
            set
            {
                if (!this.LastSeenDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.lastSeenDate = value;
            }
        }

        public void CreateBackup()
        {
            List<string> entryNameArray;
            string entryName;
            string entryPath;

            this.PrepareOnDisc();
            this.ReadFromDisc();

            this.LastSeenDate = DateTime.UtcNow;

            // read Password for Source
            entryNameArray = this.Path.Split("\\").ToList<string>();
            
            entryName = entryNameArray.ElementAt(entryNameArray.Count - 1);
            entryNameArray.RemoveAt(entryNameArray.Count - 1);
            entryPath = string.Empty;
            entryPath = string.Join("\\", entryNameArray.ToArray());

            if (entryName == string.Empty)
            {
                entryName = this.Backup.Name;
            }

            KeePassEntry keePassEntry = this.Backup.KeePassDatabase.GetEntry(entryName, entryPath);
            this.Password = keePassEntry.Password;
            this.WriteToDisc();

            BackupSourceFileCollection backupSourceFileCollection;

            backupSourceFileCollection = new BackupSourceFileCollection(this.Configuration, this);
            backupSourceFileCollection.ReadFileList();
            backupSourceFileCollection.CreateBackup();

            this.WriteToDisc();
            
            backupSourceFileCollection = null;
            GC.Collect();
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

        public void ReadFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSource_ReadFromDiscByPath, "read Id for Source " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSource_ReadFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.Backup.BackupId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourcePath>", this.Path);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSource_ReadFromDiscByPath, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.SourceId = Convert.ToInt32(dataRow[0].ToString());
                this.LastSeenDate = Convert.ToDateTime(dataRow[1].ToString());
            }
        }

        public void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSource_WriteToDisc, "write properties for Source " + this.FullRelativePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSource_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.SourceId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceLastSeenDate>", ((DateTimeOffset)this.LastSeenDate.ToLocalTime()).ToUnixTimeSeconds().ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSource_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
