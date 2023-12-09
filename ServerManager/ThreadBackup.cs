namespace blog.dachs.ServerManager
{
    using System.Data;

    public class ThreadBackup : ThreadWorker
    {
        private BackupCollection backupCollection;

        public ThreadBackup(Configuration configuration) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadBackup_ThreadBackup, "create new ThreadBackup"));
            this.backupCollection = new BackupCollection(configuration);
        }

        public BackupCollection BackupCollection
        {
            get { return this.backupCollection; }
            set { this.backupCollection = value; }
        }

        public override void Work()
        {
            //while (true)
            //{
            //    this.BackupCollection.CreateBackup();

            //    Thread.Sleep(120000);
            //}
        }
    }
}
