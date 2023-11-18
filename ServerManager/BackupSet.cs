namespace blog.dachs.ServerManager
{
    using System.Data;

    public class BackupSet : GlobalExtention
    {
        private int backupSetId;
        private BackupSource backupSource;
        private string path;
        private string name;

        public BackupSet(Configuration configuration, BackupSource backupSource) : base(configuration)
        {
            this.backupSetId = 0;
            this.BackupSource = backupSource;
        }

        public int BackupSetId
        {
            get { return this.backupSetId; }
            set { this.backupSetId = value; }
        }

        public BackupSource BackupSource
        {
            get { return this.backupSource; }
            set
            {
                if (this.BackupSource != null && !this.BackupSource.Equals(value))
                {
                    this.Changed = true;
                }

                this.backupSource = value;
            }
        }

        public string Path
        {
            get { return this.path; }
            set
            {
                if (this.Path != null && !this.Path.Equals(value))
                {
                    this.Changed = true;
                }

                this.path = value;
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.Name != null && !this.Name.Equals(value))
                {
                    this.Changed = true;
                }

                this.name = value;

                Directory.CreateDirectory(this.FullAbsolutePath);
            }
        }

        public string FullAbsolutePath
        {
            get
            {
                string fullAbsolutePath = this.BackupSource.Backup.DestinationDevicePath + "\\" + this.BackupSource.Backup.DestinationBasePath + "\\" + this.Path;
  
                if (this.Name != string.Empty)
                {
                    fullAbsolutePath = fullAbsolutePath + "\\" + this.Name.Substring(0, 4) + "\\" + this.Name.Substring(4, 2) + "\\" + this.Name.Substring(6, 2) + "\\" + this.Name.Substring(8, 2) + "\\" + this.Name.Substring(10, 2);
                }

                return fullAbsolutePath;
            }
            set { }
        }
    }
}
