using System.Data;

namespace blog.dachs.ServerManager.Backup
{
    public class BackupSourceCollection : GlobalExtention
    {
        private Backup backup;

        public BackupSourceCollection(Configuration configuration, Backup backup) : base(configuration)
        {
            this.backup = backup;
        }

        private Backup Backup
        {
            get { return this.backup; }
            set { this.backup = value; }
        }

        public void CreateBackup()
        {
            Int32 i;
            Int32 j;
            BackupSource backupSource;

            List<String> baseDirectoryCollection = new List<String>();
            List<String> sourceDirectoryCollection = new List<String>();

            // read all directories for required depth; start on baseDirectory
            sourceDirectoryCollection.Add(this.Backup.SourceBasePathWithDevice);

            // read directories in BaseDirectory
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceCollection_CreateBackup_ReadFileList, "reading subdirectories from " + this.Backup.SourceBasePathWithDevice));

            String cmdCommandOriginal = this.Database.GetCommand(Command.BackupSourceCollection_CreateBackup_ReadFileList);

            for (i = 0; i < this.Backup.SourceNameDepth; i++)
            {
                baseDirectoryCollection.Clear();
                baseDirectoryCollection.AddRange(sourceDirectoryCollection);
                sourceDirectoryCollection.Clear();

                foreach (String baseDirectory in baseDirectoryCollection)
                {
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSourceCollection_CreateBackup_ReadFileList, "reading subdirectories in depth " + this.Backup.SourceNameDepth.ToString() + " from " + baseDirectory));

                    String cmdCommandReplace = cmdCommandOriginal;
                    cmdCommandReplace = cmdCommandReplace.Replace("<SourceDirectory>", baseDirectory);

                    ProcessOutput cmdOutput = this.PowerShell.ExecuteCommand(cmdCommandReplace);

                    j = 0;
                    while (true)
                    {
                        String sourceDirectory = this.CommandLine.GetProcessOutput(cmdOutput, j);

                        if (sourceDirectory != null)
                        {
                            // check for erroneous output (mainly change of code page)
                            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(baseDirectory, sourceDirectory));
                            if (directoryInfo.Exists)
                            {
                                sourceDirectoryCollection.Add(Path.Combine(baseDirectory, sourceDirectory));
                            }
                        }
                        else
                        {
                            break;
                        }

                        j++;
                    }

                    this.CommandLine.DeleteProcessOutput(cmdOutput);
                }

                i++;
            }

            baseDirectoryCollection.Clear();

            foreach (String directoryPath in sourceDirectoryCollection)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                backupSource = new BackupSource(this.Configuration, this.Backup);
                backupSource.Backup = backup;

                if (directoryInfo.FullName == backup.SourceBasePathWithDevice)
                {
                    backupSource.Path = String.Empty;
                }
                else
                {
                    backupSource.Path = directoryInfo.FullName.Replace(backup.SourceBasePathWithDevice + "\\", String.Empty);
                }

                backupSource.CreateBackup();
            }

            sourceDirectoryCollection.Clear();
            backupSource = null;

            GC.Collect();
        }
    }
}
