using System;
using System.Data;
using System.Diagnostics;
using System.IO;

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
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:GenPw -count:1 -profile:\"ServerManager\"";

            List<string> result = this.CommandLine.Command(processStartInfo);
            
            if (result.Count == 2 && result[1].StartsWith("OK:"))
            {
                return result[0];
            }

            throw new Exception("GeneratePassword failed");
        }

        public List<string> GetListOfEntries(string title, string path = "")
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            List<string> kPScriptOutput = this.CommandLine.Command(processStartInfo);
            List<string> result = new List<string>();

            // search for existing entry
            foreach (string line in kPScriptOutput)
            {
                if (line.StartsWith("S: Title = "))
                {
                    result.Add(line.Substring(11));
                }
            }

            return result;
        }

        public KeePassEntry GetEntry(string title, string path = "")
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = this.FullAbsoluteKPScriptPath; ;
            processStartInfo.Arguments = "-c:ListEntries \"" + this.FullAbsoluteDatabasePath + "\" -pw:" + this.Password.Decrypt() + "  -ref-Title:\"" + title + "\" -refx-GroupPath:\"" + path + "\"";

            List<string> result = this.CommandLine.Command(processStartInfo);

            KeePassEntry keePassEntry = new KeePassEntry("", path);

            // search for existing entry
            foreach(string line in result)
            {
                if (line.StartsWith("S: Password = ")) { keePassEntry.Password = line.Substring(14).Encrypt(); } else 
                if (line.StartsWith("S: Title = ")) { keePassEntry.Title = line.Substring(11); }
            }

            // if empty, create new one
            if(keePassEntry.Title == string.Empty)
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
            
            this.CommandLine.Command(processStartInfo);
        }
    }
}
