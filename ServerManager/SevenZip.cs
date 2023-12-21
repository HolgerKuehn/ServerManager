namespace blog.dachs.ServerManager
{
    using System.Diagnostics;
    using System.Text;
    using blog.dachs.ServerManager.Backup;

    public class SevenZip : GlobalExtention
    {
        public SevenZip(Configuration configuration) : base(configuration)
        {
        }

        public ProcessOutput Compress(BackupSet backupSet)
        {
            ProcessOutput processOutput = new ProcessOutput(0);

            // write FileList
            DirectoryInfo directoryInfo = new DirectoryInfo(backupSet.FullAbsolutePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            
            FileInfo fileInfo = new FileInfo(backupSet.FullAbsolutePath + "\\FileList.txt");
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            FileStream filestream = File.Open(fileInfo.FullName, FileMode.Create);

            using (StreamWriter sw = new StreamWriter(filestream, Encoding.UTF8))
            {
                foreach(BackupSetSourceFile backupSetSourceFile in backupSet.SetSourceFileCollection)
                {
                    sw.WriteLine(backupSetSourceFile.SourceFile.FullAbsolutePath);
                }
            }


            // run 7zip
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zip", "7z.exe");
            processStartInfo.WorkingDirectory = backupSet.FullAbsolutePath;
            processStartInfo.Arguments  = " a -t7z -mx9 -mmt=8 -spf2 -scsUTF-8 -v1g";
            
            if (backupSet.Source.Password != string.Empty)
            {
                processStartInfo.Arguments += " -mhe -p" + backupSet.Source.Password.Decrypt();
            }
            
            processStartInfo.Arguments += " " + backupSet.Name + ".7z @FileList.txt";

            processOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            // delete FileList
            File.Delete(backupSet.FullAbsolutePath + "\\FileList.txt");

            return processOutput;
        }

        public BackupSetSourceFileCollection FilesInArchive(BackupSet backupSet)
        {
            List<string> filesInArchive = new List<string>();
            BackupSetSourceFileCollection setSourceFileCollection = new BackupSetSourceFileCollection(this.Configuration, backupSet);

            // get list of files in 
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zip", "7z.exe");
            processStartInfo.WorkingDirectory = backupSet.FullAbsolutePath;
            processStartInfo.Arguments = " l \"" + backupSet.FullAbsolutePath + "\\" + backupSet.Name + ".7z.001\"";
          
            if (backupSet.Source.Password != string.Empty)
            {
                processStartInfo.Arguments += " -p" + backupSet.Source.Password.Decrypt();
            }

            processStartInfo.Arguments += " -sccUTF-8 -slt";

            ProcessOutput processOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            int i = 0;
            while (true)
            {
                string outputLine = this.CommandLine.GetProcessOutput(processOutput, i, "a.ProcessOutput_Text like 'Path = %'");

                if (outputLine != null)
                {
                    foreach (BackupSetSourceFile setSourceFile in backupSet.SetSourceFileCollection)
                    {
                        BackupSourceFile sourceFile = setSourceFile.SourceFile;

                        if (sourceFile.FullAbsolutePath.Substring(3).Equals(outputLine.Substring(7), StringComparison.CurrentCulture))
                        {
                            setSourceFileCollection.Add(setSourceFile);
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

            return setSourceFileCollection;
        }

        public bool TestArchive(BackupSet backupSet)
        {
            bool result = false;

            // get list of files in 
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zip", "7z.exe");
            processStartInfo.WorkingDirectory = backupSet.FullAbsolutePath;
            processStartInfo.Arguments = " t \"" + backupSet.FullAbsolutePath + "\\" + backupSet.Name + ".7z.001\"";

            if (backupSet.Source.Password != string.Empty)
            {
                processStartInfo.Arguments += " -p" + backupSet.Source.Password.Decrypt();
            }

            ProcessOutput processOutput = this.CommandLine.ExecuteCommand(processStartInfo);
            string outputLine = this.CommandLine.GetProcessOutput(processOutput, 0, "a.ProcessOutput_Text like '%Everything is Ok%'");
            if (outputLine != null)
            {
                result = true;
            }

            this.CommandLine.DeleteProcessOutput(processOutput);

            return result;
        }
    }
}
