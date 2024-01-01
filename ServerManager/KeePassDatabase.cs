using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Security;

namespace blog.dachs.ServerManager
{
    public class KeePassDatabase : GlobalExtention
    {
        private int id;
        private string name;
        private string password;

        public KeePassDatabase(Configuration configuration) : base(configuration)
        {
            this.Id = 0;
        }

        public KeePassDatabase(Configuration configuration, int id) : base(configuration)
        {
            this.Id = id;

            // read set properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.KeePassDatabase_KeePassDatabase, "reading KeePass database with id " + this.Id.ToString()));

            string sqlCommand = this.Database.GetCommand(Command.KeePassDatabase_KeePassDatabase);
            sqlCommand = sqlCommand.Replace("<KeePassDatabaseID>", id.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.KeePassDatabase_KeePassDatabase, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string name;
            string password;

            // get ID and Password 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                name = dataRow[0].ToString();
                password = dataRow[1].ToString();

                if (name != null)
                    this.Name = name;

                if (password != null && password != string.Empty)
                {
                    this.Password = password;

                    try
                    {
                        password.Decrypt();
                    }
                    catch (ArgumentException ex)
                    {
                        if (ex.Source == "System.Security.Cryptography.Cng")
                        {
                            this.Password = password.Encrypt();
                            this.WriteToDisc();
                        }
                    }
                }
            }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string FullAbsoluteDatabasePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "blog.dachs", "ServerManager", this.Name); }
            set { }
        }

        public string FullAbsoluteKPScriptPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KeePass", "KPScript.exe"); }
            set { }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        private void WriteToDisc()
        {
            string sqlCommand;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.KeePassDatabase_WriteToDisc, "write properties for KeePassDatabase with ID " + this.Id + " to disc"));
            sqlCommand = this.Database.GetCommand(Command.KeePassDatabase_WriteToDisc);

            sqlCommand = sqlCommand.Replace("<KeePassDatabaseID>", this.Id.ToString());
            sqlCommand = sqlCommand.Replace("<KeePassDatabasePassword>", this.Password.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.KeePassDatabase_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }

        public string GeneratePassword()
        {
            string sqlCommandOriginal = string.Empty;
            string sqlCommandProcess = string.Empty;
            string? password;
            string? result;

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

        public List<string> GetListOfEntries(string title, string path = "")
        {
            string outputLine;
            List<string> listOfEntries = new List<string>();
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            // search for existing entry
            int row = 0;
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

        public KeePassEntry GetEntry(string title, string path = "")
        {
            string? outputLine;
            KeePassEntry keePassEntry = new KeePassEntry("", path);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            ProcessOutput kPScriptOutput = this.CommandLine.ExecuteCommand(processStartInfo);

            // search for existing entry
            int row = 0;
            while (true)
            {
                outputLine = this.CommandLine.GetProcessOutput(kPScriptOutput, row, "(a.ProcessOutput_Text like 'S: Password = %' or a.ProcessOutput_Text like 'S: Title = %')");
                
                if (outputLine != null && outputLine.StartsWith("S: Password = "))
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
            if (keePassEntry.Title == string.Empty)
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
