using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography;

namespace blog.dachs.ServerManager
{
    public class KeePassDatabase : GlobalExtention
    {
        private Int32 id;
        private String name;
        private String password;

        public KeePassDatabase(Configuration configuration) : base(configuration)
        {
            this.Id = 0;
        }

        public KeePassDatabase(Configuration configuration, Int32 id) : base(configuration)
        {
            this.Id = id;

            // read set properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.KeePassDatabase_KeePassDatabase, "reading KeePass database with id " + this.Id.ToString()));

            String sqlCommand = this.Database.GetCommand(Command.KeePassDatabase_KeePassDatabase);
            sqlCommand = sqlCommand.Replace("<KeePassDatabaseID>", id.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.KeePassDatabase_KeePassDatabase, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            String name;
            String password;

            // get ID and Password 
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                name = dataRow[0].ToString();
                password = dataRow[1].ToString();

                if (name != null)
                    this.Name = name;

                if (password != null && password != String.Empty)
                {
                    this.Password = password;

                    if(!this.Password.Contains("b64-"))
                    {
                        this.Password = password.Encrypt ();
                        this.WriteToDisc ();
                    }

                    try
                    {
                        this.Password.Decrypt();
                    }
                    catch (CryptographicException ex)
                    {
                    //    if(ex.Message == "The input data is not a complete block.") {
                    //        this.Password = password.Encrypt();
                    //        this.WriteToDisc();
                    //    }
                    }
                    //catch (ArgumentException ex)
                    //{
                    //    if (ex.Message == "cipherText is not valid" || ex.Source == "System.Security.Cryptography.Cng")
                    //    {
                    //    }
                    //}
                }
            }
        }

        public Int32 Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public String FullAbsoluteDatabasePath
        {
            get
            {
                String path = String.Empty;

                if(this.Name != null)
                {
                    path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData), "blog", "dachs", "ServerManager", this.Name);
                }
                
                return path;
            }
            set { }
        }

        public String FullAbsoluteKPScriptPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KeePass", "KPScript.exe"); }
            set { }
        }

        public String Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        private void WriteToDisc()
        {
            String sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.KeePassDatabase_WriteToDisc, "write properties for KeePassDatabase with ID " + this.Id + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.KeePassDatabase_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<KeePassDatabaseID>", this.Id.ToString());
            sqlCommand = sqlCommand.Replace("<KeePassDatabasePassword>", this.Password.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.KeePassDatabase_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        public String GeneratePassword()
        {
            String sqlCommandOriginal = String.Empty;
            String sqlCommandProcess = String.Empty;
            String? password;
            String? result;

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:GenPw -count:1 -profile:\"ServerManager\"";

            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);
            
            password = this.CommandLine.GetProcessOutput(kPScriptOutput, 0);
            result = this.CommandLine.GetProcessOutput(kPScriptOutput, 1);
            
            this.CommandLine.DeleteProcessOutput(kPScriptOutput);

            if (password != null && result != null && result.StartsWith("OK:"))
            {
                return password;
            }

            throw new Exception("GeneratePassword failed");
        }

        public List<String> GetListOfEntries(String title, String path = "")
        {
            String outputLine;
            List<String> listOfEntries = new List<String>();
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            // search for existing entry
            Int32 row = 0;
            while (true)
            {
                outputLine = this.CommandLine.GetProcessOutput(kPScriptOutput, row, "a.ProcessOutput_Text like 'S: Title = %'");

                if (outputLine != null)
                {
                    listOfEntries.Add(outputLine.Substring(11));
                }
                else if (outputLine == null)
                {
                    break;
                }

                row++;
            }

            this.CommandLine.DeleteProcessOutput(kPScriptOutput);

            return listOfEntries;
        }

        public KeePassEntry GetEntry(String title, String path = "")
        {
            String? outputLine;
            KeePassEntry keePassEntry = new KeePassEntry("", path);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            // search for existing entry
            Int32 row = 0;
            while (true)
            {
                outputLine = this.CommandLine.GetProcessOutput(kPScriptOutput, row, "(a.ProcessOutput_Text like 'S: UserName = %' or a.ProcessOutput_Text like 'S: Password = %' or a.ProcessOutput_Text like 'S: Title = %')");

                if (outputLine != null && outputLine.StartsWith("S: UserName = "))
                {
                    keePassEntry.UserName = outputLine.Substring(14).Encrypt();
                }
                else if (outputLine != null && outputLine.StartsWith("S: Password = "))
                {
                    keePassEntry.Password = outputLine.Substring(14).Encrypt();
                }
                else if (outputLine != null && outputLine.StartsWith("S: Title = "))
                {
                    keePassEntry.Title = outputLine.Substring(11);
                }
                else if (outputLine == null)
                {
                    break;
                }

                row++;
            }

            this.CommandLine.DeleteProcessOutput(kPScriptOutput);


            // if empty, create new one
            if (keePassEntry.Title == String.Empty)
            {
                keePassEntry.Title = title;
                keePassEntry.Password = this.GeneratePassword().Encrypt();

                this.CreateEntry(keePassEntry);
                Thread.Sleep(2000);
            }

            return keePassEntry;
        }

        public void CreateEntry(KeePassEntry keePassEntry)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:AddEntry \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -Title:\"" + keePassEntry.Title + "\" -UserName:\"\" -Password:\"" + keePassEntry.Password.Decrypt() + "\"";
            
            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);
            this.CommandLine.DeleteProcessOutput(kPScriptOutput);
        }
    }
}
