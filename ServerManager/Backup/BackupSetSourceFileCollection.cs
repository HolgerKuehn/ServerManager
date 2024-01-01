namespace blog.dachs.ServerManager.Backup
{
    public class BackupSetSourceFileCollection : GlobalExtention
    {
        private BackupSet backupSet;

        public BackupSetSourceFileCollection(Configuration configuration, BackupSet backupSet) : base(configuration)
        {
            this.BackupSet = backupSet;
        }

        private BackupSet BackupSet
        {
            get { return this.backupSet; }
            set { this.backupSet = value; }
        }

        public void AddBackupSetFile(BackupSourceFile sourceFile)
        {
            BackupSetSourceFile backupSetSourceFile;

            backupSetSourceFile = new BackupSetSourceFile(this.Configuration, this.BackupSet, sourceFile);
            backupSetSourceFile.PrepareOnDisc();
            backupSetSourceFile.ReadFromDisc();
            backupSetSourceFile.WriteToDisc();
        }
    }
}
