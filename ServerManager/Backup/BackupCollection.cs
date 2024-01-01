namespace blog.dachs.ServerManager.Backup
{
    using System;
    using System.Data;

    public class BackupCollection : GlobalExtention
    {
        public BackupCollection(Configuration configuration) : base(configuration)
        {
        }

        public void CreateBackup()
        {
            // read backups from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupCollection_BackupCollection, "reading BackupCollection"));

            string sqlCommand = this.Database.GetCommand(Command.BackupCollection_BackupCollection);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupCollection_BackupCollection, sqlCommand));

            int i = 0;
            while (true)
            {
                DataRow dataRow = this.Database.GetDataRow(sqlCommand, i);

                if (dataRow != null)
                {
                    int backupId = Convert.ToInt32(dataRow[0].ToString());
                    Backup backup = new Backup(this.Configuration, backupId);

                    backup.CreateBackup();
                }
                else
                { 
                    break;
                }

                i++;
            }
        }
    }
}