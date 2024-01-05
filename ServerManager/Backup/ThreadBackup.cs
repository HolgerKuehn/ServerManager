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
            while (true)
            {
                this.BackupCollection.CreateBackup();

                Thread.Sleep(1000);
            }
        }
    }
}
