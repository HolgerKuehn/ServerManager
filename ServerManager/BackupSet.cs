namespace blog.dachs.ServerManager
{
    using System.Data;

    public class BackupSet : GlobalExtention
    {
        private int setId;
        private BackupSource source;
        private string path;
        private string name;
        private DateTime backupDate;
        private BackupSetSourceFileCollection setSourceFileCollection;

        public BackupSet(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.SetId = 0;
            this.Source = backupSource;
            this.SetSourceFileCollection = new BackupSetSourceFileCollection(this.Configuration, this);
        }

        public int SetId
        {
            get { return this.setId; }
            set { this.setId = value; }
        }

        public BackupSource Source
        {
            get { return this.source; }
            set
            {
                if (this.Source != null && !this.Source.Equals(value))
                {
                    this.Changed = true;
                }

                this.source = value;
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

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.Name != null && !this.Name.Equals(value))
                {
                    this.Changed = true;
                }

                this.name = value;
            }
        }

        public string FullAbsolutePath
        {
            get
            {
                string fullAbsolutePath = this.Source.Backup.DestinationDevicePath + "\\" + this.Source.Backup.DestinationBasePath + "\\" + this.Path;
  
                if (this.Name != string.Empty)
                {
                    fullAbsolutePath = fullAbsolutePath + "\\" + this.Name.Substring(0, 4) + "\\" + this.Name.Substring(4, 2) + "\\" + this.Name.Substring(6, 2) + "\\" + this.Name.Substring(8, 2) + "\\" + this.Name.Substring(10, 2);
                }

                return fullAbsolutePath;
            }
            set { }
        }

        public DateTime BackupDate
        {
            get { return this.backupDate; }
            set
            {
                if (!this.BackupDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.backupDate = value;
            }
        }

        public BackupSetSourceFileCollection SetSourceFileCollection
        {
            get { return this.setSourceFileCollection; }
            set { this.setSourceFileCollection = value; }
        }

        public void CreateBackup()
        {
            if(this.Source.SourceFileCollection.GetFilesToBackup().Count > 0)
            {
                this.PrepareOnDisc();
                this.ReadFromDisc();

                foreach (BackupSourceFile sourceFile in this.Source.SourceFileCollection.GetFilesToBackup())
                {
                    BackupSetSourceFile setSourceFile;

                    setSourceFile = new BackupSetSourceFile(this.Configuration, this, sourceFile);
                    setSourceFile.CreateBackup();

                    this.SetSourceFileCollection.Add(setSourceFile);
                }

                // create 7z-File
                this.BackupDate = DateTime.UtcNow;
                this.SevenZip.Compress(this);

                // validate files included in archive
                foreach (BackupSetSourceFile setSourceFile in this.SevenZip.FilesInArchive(this))
                {
                    setSourceFile.SourceFile.LastBackupDate = this.BackupDate;
                    setSourceFile.SourceFile.WriteToDisc();

                    setSourceFile.State = BackupSetSourceFileState.includedInSet;
                    setSourceFile.WriteToDisc();
                }

                // test archive
                if (this.SevenZip.TestArchive(this))
                {
                    foreach (BackupSetSourceFile setSourceFile in this.SetSourceFileCollection)
                    {
                        if (setSourceFile.State == BackupSetSourceFileState.includedInSet)
                        {
                            setSourceFile.State = BackupSetSourceFileState.validated;
                            setSourceFile.WriteToDisc();
                        } 
                    }
                }

                this.WriteToDisc();
            }
        }

        private void PrepareOnDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_PrepareOnDisc, "create SetId for " + this.FullAbsolutePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSet_PrepareOnDisc);
            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.Source.SourceId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetPath>", this.Path);
            sqlCommand = sqlCommand.Replace("<BackupSetName>", this.Name);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_PrepareOnDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        private void ReadFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_ReadFromDisc, "read Id for Set " + this.FullAbsolutePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSet_ReadFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.Source.SourceId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetPath>", this.Path);
            sqlCommand = sqlCommand.Replace("<BackupSetName>", this.Name);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_ReadFromDisc, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.SetId = Convert.ToInt32(dataRow[0].ToString());
            }
        }

        private void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_WriteToDisc, "write properties for set " + this.FullAbsolutePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSet_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetTimestamp>", ((DateTimeOffset)this.BackupDate.ToLocalTime()).ToUnixTimeSeconds().ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
