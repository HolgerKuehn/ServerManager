namespace blog.dachs.ServerManager
{
	public class Backup : GlobalExtention
	{ 
		private int backupId;
		private string name;
		private string sourceBasePath;
		private string destinationDevicePath;
		private string destinationBasePath;
		private string keePassFile;

        public Backup(Configuration configuration) : base(configuration)
        {
        }

        public int BackupId
		{
			get { return this.backupId; }
			set { this.backupId = value; }
		}

        public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

        public string SourceBasePath
		{
			get { return this.sourceBasePath; }
			set { this.sourceBasePath = value; }
		}

        public string DestinationDevicePath
		{
			get { return this.destinationDevicePath; }
			set { this.destinationDevicePath = value; }
		}

        public string DestinationBasePath
		{
			get { return this.destinationBasePath; }
			set { this.destinationBasePath = value; }
		}

        public string KeePassFile
		{
			get { return this.keePassFile; }
			set { this.keePassFile = value; }
		}
    }
}
