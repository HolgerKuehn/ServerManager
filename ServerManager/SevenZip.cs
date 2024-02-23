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

        public ProcessOutput FilesInArchive(BackupSet backupSet)
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

            return this.CommandLine.ExecuteCommand(processStartInfo);
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
