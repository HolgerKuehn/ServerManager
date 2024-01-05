namespace blog.dachs.ServerManager.Backup
{
    using System;
    using System.Data;

    public class BackupSourceFileCollection : GlobalExtention
    {
        private BackupSource backupSource;

        public BackupSourceFileCollection(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.BackupSource = backupSource;
        }

        private BackupSource BackupSource
        {
            get { return this.backupSource; }
            set { this.backupSource = value; }
        }

        public void ReadFileList()
        {
            string cmdCommand;
            FileInfo fileInfo;
            BackupSourceFile sourceFile;
            string path;
            string name;

            // read files in directory
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFileCollection_ReadFileList, "reading file from " + this.BackupSource.FullAbsolutePath));

            cmdCommand = this.Database.GetCommand(Command.BackupSourceFileCollection_ReadFileList);
            cmdCommand = cmdCommand.Replace("<SourceDirectory>", this.BackupSource.FullAbsolutePath);
            ProcessOutput cmdOutput = this.PowerShell.ExecuteCommand(cmdCommand);

            int i = 0;
            while (true)
            {
                string sourceFilePath = this.CommandLine.GetProcessOutput(cmdOutput, i);
            
                if (sourceFilePath != null && sourceFilePath.Trim() != string.Empty)
                {
                    path = string.Empty;
                    name = string.Empty;

                    fileInfo = new FileInfo(sourceFilePath);

                    if (fileInfo.Exists)
                    {
                        if (fileInfo.DirectoryName != null && fileInfo.DirectoryName != this.BackupSource.FullAbsolutePath)
                        {
                            path = fileInfo.DirectoryName.Replace(BackupSource.FullAbsolutePath + "\\", string.Empty);
                        }

                        name = fileInfo.Name;

                        sourceFile = new BackupSourceFile(this.Configuration, this.backupSource, path, name);

                        // set additional data
                        sourceFile.CreationDate = fileInfo.CreationTime;
                        sourceFile.LastAccessDate = fileInfo.LastAccessTime;
                        sourceFile.LastWriteDate = fileInfo.LastWriteTime;
                        sourceFile.LastSeenDate = DateTime.Now;
                        sourceFile.Size = (ulong)fileInfo.Length;

                        sourceFile.PrepareOnDisc();
                        sourceFile.ReadFromDisc();
                        sourceFile.WriteToDisc();
                        sourceFile = null;
                    }
                }
                else
                {
                    break;
                }
            
                i++;
            }
            
            this.CommandLine.DeleteProcessOutput(cmdOutput);
            GC.Collect();
        }

        public void CreateBackup()
        {
            int i;
            int sourceFileId;
            string sourceFilePath;
            string sourceFileName;
            ulong sourceFileSize;

            BackupSourceFile backupSourceFile;

            BackupSetCollection backupSetCollection = new BackupSetCollection(this.Configuration, this.BackupSource);

            if (backupSetCollection.Set.State < BackupSetState.FileList_created)
            { 
                // read changed files since last backup
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceFileCollection_CreateBackup, "reading BackupSourceFiles with changes"));

                string cmdCommand = this.Database.GetCommand(Command.BackupSourceFileCollection_CreateBackup);
                cmdCommand = cmdCommand.Replace("<BackupSourceID>", this.BackupSource.SourceId.ToString());

                i = 0;
                while (true)
                {
                    DataRow dataRow = this.Database.GetDataRow(cmdCommand, i);

                    if (dataRow != null)
                    {
                        sourceFileId = Convert.ToInt32(dataRow[0].ToString());
                        sourceFilePath = dataRow[1].ToString();
                        sourceFileName = dataRow[2].ToString();
                        sourceFileSize = Convert.ToUInt64(dataRow[3].ToString());

                        if (sourceFilePath != null && sourceFileName != null)
                        {
                            backupSourceFile = new BackupSourceFile(this.Configuration, this.BackupSource);
                            backupSourceFile.BackupSourceFileId = sourceFileId;
                            backupSourceFile.Path = sourceFilePath;
                            backupSourceFile.Name = sourceFileName;
                            backupSourceFile.Size = sourceFileSize;
                            backupSetCollection.CreateBackup(backupSourceFile);
                        }
                    }
                    else
                    {
                        break;
                    }

                    backupSourceFile = null;
                    GC.Collect();

                    i++;
                }
            }

            if (backupSetCollection.Set.State == BackupSetState.creating_FileList)
            {
                backupSetCollection.Set.State = BackupSetState.FileList_created;
            }

            backupSetCollection.Set.WriteToDisc();

            backupSetCollection.CreateBackup();
        }
    }
}
