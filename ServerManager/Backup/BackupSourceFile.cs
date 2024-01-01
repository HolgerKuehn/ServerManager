using System.Data;

namespace blog.dachs.ServerManager.Backup
{
    public class BackupSourceFile : GlobalExtention
    {
        private int backupSourceFileId;
        private BackupSource backupSource;
        private string path;
        private string name;
        private DateTime creationDate;
        private DateTime lastAccessDate;
        private DateTime lastWriteDate;
        private DateTime lastSeenDate;
        private DateTime lastBackupDate;
        private ulong size;

        public BackupSourceFile(Configuration configuration, BackupSource backupSource, string path, string name) : base(configuration)
        {
            this.backupSourceFileId = 0;

            this.BackupSource = backupSource;
            this.Path = path;
            this.Name = name;

            this.CreationDate = DateTime.MinValue;
            this.lastAccessDate = DateTime.MinValue;
            this.lastWriteDate = DateTime.MinValue;
            this.lastSeenDate = DateTime.MinValue;
            this.lastBackupDate = DateTime.MinValue;
            this.size = 0;
        }

        public BackupSourceFile(Configuration configuration, BackupSource backupSource) : this(configuration, backupSource, "", "")
        {
        }

        public int BackupSourceFileId
        {
            get { return this.backupSourceFileId; }
            set { this.backupSourceFileId = value; }
        }

        public BackupSource BackupSource
        {
            get { return this.backupSource; }
            set
            { 
                if (this.BackupSource != null && !this.BackupSource.Equals(value))
                {
                    this.Changed = true;
                }

                this.backupSource = value;
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
                string path;

                path = this.BackupSource.FullAbsolutePath;

                if (this.Path != string.Empty)
                {
                    path += "\\" + this.Path;
                }

                path += "\\" + this.Name;

                return path;
            }
            set { }
        }

        public string FullRelativePath
        {
            get
            {
                string path;

                path = this.BackupSource.FullRelativePath;

                if (this.Path != string.Empty)
                {
                    path += "\\" + this.Path;
                }

                path += "\\" + this.Name;

                return path;
            }
            set { }
        }

        public DateTime CreationDate
        {
            get { return this.creationDate; }
            set
            {
                if (!this.CreationDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.creationDate = value;
            }
        }

        public DateTime LastAccessDate
        {
            get { return this.lastAccessDate; }
            set
            {
                if (!this.LastAccessDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.lastAccessDate = value;
            }
        }

        public DateTime LastWriteDate
        {
            get { return this.lastWriteDate; }
            set
            {
                if (!this.LastWriteDate.Equals(value))
                {
                    this.Changed = true;
                }

                this.lastWriteDate = value;
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

        public DateTime LastBackupDate
        {
            get { return this.lastBackupDate; }
        }

        public ulong Size
        {
            get { return this.size; }
            set
            {
                if (!this.Size.Equals(value))
                {
                    this.Changed = true;
                }

                this.size = value;
            }
        }

        public void PrepareOnDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_PrepareOnDisc, "create BackupSourceFileId for " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_PrepareOnDisc);
            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.BackupSource.SourceId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFilePath>", this.Path.Replace("'", "''"));
            sqlCommand = sqlCommand.Replace("<BackupSourceFileName>", this.Name.Replace("'", "''"));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSourceFile_PrepareOnDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        public void ReadFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_ReadFromDisc, "read BackupSourceFileId for " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_ReadFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.BackupSource.SourceId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFilePath>", this.Path.Replace("'", "''"));
            sqlCommand = sqlCommand.Replace("<BackupSourceFileName>", this.Name.Replace("'", "''"));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSourceFile_ReadFromDisc, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.BackupSourceFileId = Convert.ToInt32(dataRow[0].ToString());
            }
        }

        public void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_WriteToDisc, "write properties for " + this.FullRelativePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSourceFileID>", this.BackupSourceFileId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileSize>", this.Size.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileCreationDate>", ((DateTimeOffset)this.CreationDate.ToLocalTime()).ToUnixTimeSeconds().ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileLastWriteDate>", ((DateTimeOffset)this.LastWriteDate.ToLocalTime()).ToUnixTimeSeconds().ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileLastAccessDate>", ((DateTimeOffset)this.LastAccessDate.ToLocalTime()).ToUnixTimeSeconds().ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFileLastSeenDate>", ((DateTimeOffset)this.LastSeenDate.ToLocalTime()).ToUnixTimeSeconds().ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSourceFile_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
