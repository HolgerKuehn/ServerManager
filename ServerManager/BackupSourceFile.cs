using System.Data;

namespace blog.dachs.ServerManager
{
    public class BackupSourceFile : GlobalExtention
    {
        private int backupFileId;
        private Backup backup;
        private string relativePath;
        private string fileName;
        private DateTime creationDate;
        private DateTime lastAccessDate;
        private DateTime lastWriteDate;
        private DateTime lastSeenDate;
        private long size;

        public BackupSourceFile(Configuration configuration) : base(configuration)
        {
        }

        public int BackupFileId
        {
            get { return this.backupFileId; }
            set { this.backupFileId = value; }
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

        public string RelativePath
        {
            get { return this.relativePath; }
            set
            {
                if (this.RelativePath != null && !this.RelativePath.Equals(value))
                {
                    this.Changed = true;
                }

                this.relativePath = value;
            }
        }

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                if (this.FileName != null && !this.FileName.Equals(value))
                {
                    this.Changed = true;
                }

                this.fileName = value;
            }
        }

        public string FullAbsolutePath
        {
            get { return this.Backup.SourceBasePath + "\\" + this.RelativePath + "\\" + this.FileName; }
            set { }
        }

        public string FullRelativePath
        {
            get { return this.RelativePath + "\\" + this.FileName; }
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

        public long Size
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

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_PrepareOnDisc, "create BackupFileId for " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_PrepareOnDisc);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.Backup.BackupId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFile_Path>", this.RelativePath);
            sqlCommand = sqlCommand.Replace("<BackupSourceFile_Name>", this.FileName);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSourceFile_PrepareOnDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        public void ReadBackupFileIdFromDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_ReadBackupFileIdFromDisc, "read BackupFileId for " + this.FullRelativePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_ReadBackupFileIdFromDisc);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.Backup.BackupId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSourceFile_Path>", this.RelativePath);
            sqlCommand = sqlCommand.Replace("<BackupSourceFile_Name>", this.FileName);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSourceFile_ReadBackupFileIdFromDisc, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];

                this.BackupFileId = Convert.ToInt32(dataRow[0].ToString());
            }
        }

        public void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFile_WriteToDisc, "write properties for " + this.FullRelativePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSourceFile_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSourceFileID>", this.BackupFileId.ToString());
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
