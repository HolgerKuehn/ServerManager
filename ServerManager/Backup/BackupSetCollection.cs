using System.Data;

namespace blog.dachs.ServerManager.Backup
{
    public class BackupSetCollection : GlobalExtention
    {
        private BackupSource backupSource;
        private BackupSet set;

        public BackupSetCollection(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.backupSource = backupSource;
            this.Set = new BackupSet(configuration, backupSource);

            // read current set, not yet validated
            int i;
            string sqlCommand;
            string path;
            string name;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupSetCollection_BackupSetCollection, "read Id for last BackupSet not yet validated for Source " + this.Source.FullAbsolutePath));
            sqlCommand = this.Database.GetCommand(Command.BackupSetCollection_BackupSetCollection);
            sqlCommand = sqlCommand.Replace("<BackupSourceID>", this.Source.SourceId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupSetCollection_BackupSetCollection, sqlCommand));

            i = 0;
            while (true)
            {
                DataRow dataRow = this.Database.GetDataRow(sqlCommand, i);

                if (dataRow != null)
                {
                    this.Set.SetId = Convert.ToInt32(dataRow[0].ToString());

                    path = dataRow[1].ToString();

                    if (path != null)
                    {
                        this.Set.Path = path;
                    }

                    name = dataRow[2].ToString();

                    if (name != null)
                    {
                        this.Set.Name = name;
                    }

                    this.Set.BackupDate = Convert.ToDateTime(dataRow[3].ToString());
                    this.Set.Size = Convert.ToUInt64(dataRow[4].ToString());
                    this.Set.State = (BackupSetState)Convert.ToByte(dataRow[5].ToString());
                }
                else
                {
                    break;
                }

                i++;
            }
        }

        private BackupSource Source
        {
            get { return this.backupSource; }
            set { this.backupSource = value; }
        }

        public BackupSet Set
        {
            get { return this.set; }
            set { this.set = value; }
        }

        public void CreateBackup()
        {
            this.Set.CreateBackup();
        }

        public void CreateBackup(BackupSourceFile sourceFile)
        {
            this.Set.CreateBackup(sourceFile);
        }
    }
}
