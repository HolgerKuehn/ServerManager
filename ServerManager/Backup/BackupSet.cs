namespace blog.dachs.ServerManager.Backup
{
    using System.Data;
    using System.Diagnostics;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;

    public enum BackupSetState : byte
    {
        initialized = 0,
        created = 1,
        creating_FileList = 2,
        FileList_created = 3,
        BackupSpace_required = 4,
        BackupSpace_available = 5,
        FileList_written = 6,
        creating_Backup = 7,
        Backup_created = 8,
        reading_FileList = 9,
        FileList_read = 10,
        validating_Backup = 11,
        Backup_validated = 12
    }

    public class BackupSet : GlobalExtention
    {
        private int setId;
        private BackupSource source;
        private string path;
        private string name;
        private DateTime backupDate;
        private ulong size;
        private BackupSetState state;
        private bool backupSetFileAdded;
        private BackupSetSourceFileCollection backupSetSourceFileCollection;

        public BackupSet(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.SetId = 0;
            this.Source = backupSource;

            DateTime date = DateTime.UtcNow;
            this.Path = this.Source.Path;
            this.Name = date.Year.ToString().PadLeft(4, '0') + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0') + date.Hour.ToString().PadLeft(2, '0') + date.Minute.ToString().PadLeft(2, '0');
            this.Size = 0;
            this.State = BackupSetState.initialized;
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

        public BackupSetState State
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

        public bool BackupSetFileAdded
        {
            get { return this.backupSetFileAdded; }
            set { this.backupSetFileAdded = value; }
        }

        private BackupSetSourceFileCollection BackupSetSourceFileCollection
        {
            get
            {
                if (this.backupSetSourceFileCollection == null)
                {
                    this.backupSetSourceFileCollection = new BackupSetSourceFileCollection(this.Configuration, this);
                }

                return this.backupSetSourceFileCollection;
            }
        }

        public void CreateBackup()
        {
            if (this.State == BackupSetState.FileList_created)
            {
                DataRow dataRow;

                string sqlCommand;
                ulong sizeBackupSet;
                ulong sizeDestinationDevice;


                // compare space available and space required
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_CompareSize, "compare size of BackupSet and Destination Device"));
                sqlCommand = this.Database.GetCommand(Command.BackupSet_CreateBackup_CompareSize);
                sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_CompareSize, sqlCommand));

                sizeBackupSet = 0;
                sizeDestinationDevice = 0;

                dataRow = this.Database.GetDataRow(sqlCommand, 0);

                if (dataRow != null)
                {
                    sizeBackupSet = Convert.ToUInt64(dataRow[0].ToString());
                    sizeDestinationDevice = Convert.ToUInt64(dataRow[1].ToString());
                }

                if (sizeBackupSet > sizeDestinationDevice)
                {
                    this.State = BackupSetState.BackupSpace_required;
                }
                else
                {
                    this.State = BackupSetState.BackupSpace_available;

                }

                this.WriteToDisc();
            }

            if (this.State == BackupSetState.BackupSpace_available)
            {
                // write FileList
                DirectoryInfo directoryInfo = new DirectoryInfo(this.FullAbsolutePath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                FileInfo fileListFileInfo = new FileInfo(this.FullAbsolutePath + "\\FileList.txt");
                if (fileListFileInfo.Exists)
                {
                    fileListFileInfo.Delete();
                }

                // write FileList
                FileStream filestream = File.Open(fileListFileInfo.FullName, FileMode.Create);
                StreamWriter fileListStreamWriter = new StreamWriter(filestream, Encoding.UTF8);
                
                string sqlCommand;

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_WriteFileList, "read FileList from Set " + this.FullAbsolutePath));
                sqlCommand = this.Database.GetCommand(Command.BackupSet_CreateBackup_WriteFileList);
                sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_WriteFileList, sqlCommand));

                // read FileList from DB
                using (fileListStreamWriter)
                {
                    int i = 0;
                    while (true)
                    {
                        DataRow dataRow = this.Database.GetDataRow(sqlCommand, i);

                        if (dataRow != null)
                        {
                            string fullPath = dataRow[0].ToString();

                            if (fullPath != null && fullPath != string.Empty)
                            { 
                                fileListStreamWriter.WriteLine(fullPath);
                            }
                        }
                        else
                        { 
                            break;
                        }

                        i++;
                    }
                }

                this.State = BackupSetState.FileList_written;
                
                this.WriteToDisc();
            }

            if (this.State == BackupSetState.FileList_written)
            {
                ProcessOutput processOutput;

                // create 7z-File
                this.State = BackupSetState.creating_Backup;
                this.WriteToDisc();

                this.BackupDate = DateTime.UtcNow;
                processOutput = this.SevenZip.Compress(this);
                this.CommandLine.DeleteProcessOutput(processOutput);

                this.State = BackupSetState.Backup_created;
                this.WriteToDisc();
            }

            if (this.State == BackupSetState.Backup_created)
            {
                this.State = BackupSetState.reading_FileList;
                this.WriteToDisc();

                string sqlCommandOriginal;
                string sqlCommandReplace;

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_ReadFileList, "change State for BackupSetSourceFile " + this.FullAbsolutePath));
                sqlCommandOriginal = this.Database.GetCommand(Command.BackupSet_CreateBackup_ReadFileList);

                // validate files included in archive
                ProcessOutput processOutput = this.SevenZip.FilesInArchive(this);

                string fileNameWithoutDevice;
                string packedSize;
                ulong size;

                size = 0;
                int i = 0;
                while (true)
                {
                    fileNameWithoutDevice = string.Empty;
                    string outputLine = this.CommandLine.GetProcessOutput(processOutput, i, "(a.ProcessOutput_Text like 'Path = %' or a.ProcessOutput_Text like 'Packed Size = %')");

                    if (outputLine != null)
                    {
                        if (outputLine.Contains("Path = "))
                        {
                            fileNameWithoutDevice = outputLine.Substring(7);

                            sqlCommandReplace = sqlCommandOriginal;
                            sqlCommandReplace = sqlCommandReplace.Replace("<BackupSetID>", this.SetId.ToString());
                            sqlCommandReplace = sqlCommandReplace.Replace("<BackupSourceFilePath>", fileNameWithoutDevice.Replace("'", "''"));

                            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_ReadFileList, sqlCommandReplace));
                            this.Database.ExecuteCommand(sqlCommandReplace);
                        }

                        if (outputLine.Contains("Packed Size = "))
                        {
                            packedSize = outputLine.Substring(13).Trim();
                            if (packedSize != string.Empty)
                            {
                                size += Convert.ToUInt64(packedSize);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }

                this.CommandLine.DeleteProcessOutput(processOutput);

                this.Size = size;
                this.State = BackupSetState.FileList_read;
                this.WriteToDisc();
            }

            if (this.State == BackupSetState.FileList_read)
            {
                this.State = BackupSetState.validating_Backup;
                this.WriteToDisc();

                string sqlCommand;

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_ValidateBackup, "validate BackupSet " + this.FullAbsolutePath));
                sqlCommand = this.Database.GetCommand(Command.BackupSet_CreateBackup_ValidateBackup);

                // test archive, possibly three times, as RClone might not provide them fast enough the first time
                int i = 0;
                bool testArchive = false;
                while (!testArchive && i < 3)
                {
                    testArchive = this.SevenZip.TestArchive(this);

                    if (testArchive)
                    {
                        sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());

                        this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_ValidateBackup, sqlCommand));
                        this.Database.ExecuteCommand(sqlCommand);

                        this.State = BackupSetState.Backup_validated;
                    }
                }

                this.WriteToDisc();
            }

            if (this.State == BackupSetState.Backup_validated)
            {
                string sqlCommand;

                // set LastBackupDate
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_ValidateBackup, "set LastBackupTimestamp for " + this.FullAbsolutePath));
                sqlCommand = this.Database.GetCommand(Command.BackupSet_CreateBackup_LastBackupTimestamp);
                sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_ValidateBackup, sqlCommand));
                this.Database.ExecuteCommand(sqlCommand);


                // set Size
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_CreateBackup_ActualSize, "set UsedSize for BackupDevice"));
                sqlCommand = this.Database.GetCommand(Command.BackupSet_CreateBackup_ActualSize);

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_CreateBackup_ValidateBackup, sqlCommand));
                this.Database.ExecuteCommand(sqlCommand);
            }
        }

        public void CreateBackup(BackupSourceFile sourceFile)
        {
            if (this.State == BackupSetState.initialized)
            {
                this.PrepareOnDisc();
                this.ReadFromDisc();

                this.BackupDate = DateTime.UtcNow;
                this.State = BackupSetState.created;

                this.WriteToDisc();
            }


            if (this.State == BackupSetState.created)
            {
                this.BackupSetFileAdded = true;
                this.State = BackupSetState.creating_FileList;

                this.WriteToDisc();
            }


            if (this.State == BackupSetState.creating_FileList)
            {
                // determine size
                this.Size += sourceFile.Size;

                this.BackupSetSourceFileCollection.AddBackupSetFile(sourceFile);

                this.WriteToDisc();
            }
        }
        
        public void PrepareOnDisc()
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

        public void ReadFromDisc()
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

        public void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSet_WriteToDisc, "write properties for set " + this.FullAbsolutePath + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.BackupSet_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<BackupSetID>", this.SetId.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetTimestamp>", ((DateTimeOffset)this.BackupDate.ToLocalTime()).ToUnixTimeSeconds().ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetSize>", this.Size.ToString());
            sqlCommand = sqlCommand.Replace("<BackupSetStateID>", ((byte)this.State).ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSet_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
