namespace blog.dachs.ServerManager.Backup
{
    public class ThreadBackup : ThreadWorker
    {
        private BackupCollection backupCollection;

        public ThreadBackup(Configuration configuration) : base(configuration)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadBackup_ThreadBackup, "create new ThreadBackup"));
            backupCollection = new BackupCollection(configuration);
        }

        public BackupCollection BackupCollection
        {
            get { return backupCollection; }
            set { backupCollection = value; }
        }

        public override void Work()
        {
            bool doBackup = true;
            DateTime dateTime;
            TimeSpan timeOfDay;
            int backupDay = 0;
            int lastBackupDay = 0;

            while (true)
            {
                while (!doBackup)
                {
                    dateTime = DateTime.Now;
                    timeOfDay = dateTime.TimeOfDay;
                    backupDay = dateTime.DayOfYear;

                    if (timeOfDay.TotalHours > 23 && backupDay != lastBackupDay)
                    {
                        doBackup = true;
                    }

                    Thread.Sleep(60000);
                }

                doBackup = false;
                lastBackupDay = backupDay;

                this.BackupCollection.CreateBackup();
            }
        }
    }
}
