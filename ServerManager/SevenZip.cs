using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq.Expressions;

namespace blog.dachs.ServerManager
{
    public class SevenZip : GlobalExtention
    {
        public SevenZip(Configuration configuration) : base(configuration)
        {
        }

        public List<string> Compress(BackupSet backupSet)
        {
            List<string> result = new List<string>();

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

            result = this.CommandLine.Command(processStartInfo);

            // delete FileList
            File.Delete(backupSet.FullAbsolutePath + "\\FileList.txt");

            return result;
        }

        public BackupSetSourceFileCollection FilesInArchive(BackupSet backupSet)
        {
            List<string> resultList = new List<string>();
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

            resultList = this.CommandLine.Command(processStartInfo);

            foreach (string line in resultList)
            {
                if (line.StartsWith("Path = "))
                {
                    foreach (BackupSetSourceFile setSourceFile in backupSet.SetSourceFileCollection)
                    {
                        BackupSourceFile sourceFile = setSourceFile.SourceFile;

                        if (sourceFile.FullAbsolutePath.Substring(3).Equals(line.Substring(7), StringComparison.CurrentCulture))
                        {
                            setSourceFileCollection.Add(setSourceFile);
                        }
                    }
                }
            }

            return setSourceFileCollection;
        }

        public bool TestArchive(BackupSet backupSet)
        {
            List<string> resultList = new List<string>();
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

            resultList = this.CommandLine.Command(processStartInfo);

            foreach (string line in resultList)
            {
                if (line.Contains("Everything is Ok"))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
