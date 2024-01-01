namespace blog.dachs.ServerManager.Backup
{
    using System.Data;

    public class Backup : GlobalExtention
	{ 
		private int backupId;
		private string name;
        private string sourceDevicePath;
        private string sourceBasePath;
        private int sourceNameDepth;
        private BackupDestinationDevice destinationDevice;
		private string destinationBasePath;
        private KeePassDatabase keePassDatabase;

        public Backup(Configuration configuration, int backupId) : base(configuration)
        {
			this.backupId = backupId;

            // read set properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.Backup_Backup, "reading Backup properties"));

            string sqlCommand = this.Database.GetCommand(Command.Backup_Backup);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.backupId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.Backup_Backup, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
			DataRow dataRow = null;
            string name;
            string sourceDevicePath;
            string sourceBasePath;
            int sourceNameDepth;
            int destinationDeviceId;
            string destinationBasePath;
            string keePassDatabase;

            // get Backup properties 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                name = dataRow[0].ToString();
                sourceDevicePath = dataRow[1].ToString();
                sourceBasePath = dataRow[2].ToString();
                sourceNameDepth = Convert.ToInt32(dataRow[3].ToString());
                destinationDeviceId = Convert.ToInt32(dataRow[4].ToString());
                destinationBasePath = dataRow[5].ToString();
                keePassDatabase = dataRow[6].ToString();

                if (name != null)
					this.Name = name;

                if (sourceDevicePath != null)
                    this.SourceDevicePath = sourceDevicePath;

                if (sourceBasePath != null)
					this.SourceBasePathWithDevice = sourceBasePath;

                this.DestinationDevice = new BackupDestinationDevice(this.Configuration, destinationDeviceId);

                this.SourceNameDepth = sourceNameDepth;

                if (destinationBasePath != null)
                    this.DestinationBasePath = destinationBasePath;

                if (keePassDatabase != null)
                    this.KeePassDatabase = this.KeePass.GetKeePassDatabase(keePassDatabase);
            }
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

        public string SourceDevicePath
        {
            get { return this.sourceDevicePath; }
            set { this.sourceDevicePath = value; }
        }

        public string SourceBasePathWithDevice
		{
			get { return Path.Combine(this.SourceDevicePath, this.sourceBasePath); }
			set { this.sourceBasePath = value; }
		}

        public string SourceBasePathWithoutDevice
        {
            get { return this.sourceBasePath; }
            set { this.sourceBasePath = value; }
        }

        public int SourceNameDepth
        {
            get { return sourceNameDepth; }
            set { this.sourceNameDepth = value; }
        }

        public BackupDestinationDevice DestinationDevice
        {
            get { return this.destinationDevice; }
            set { this.destinationDevice = value; }
        }

        public string DestinationDevicePath
		{
			get { return this.DestinationDevice.Path; }
			set { this.DestinationDevice.Path = value; }
		}

        public string DestinationBasePath
		{
			get { return this.destinationBasePath; }
			set { this.destinationBasePath = value; }
		}

        public string FullAbsolutePath
        {
            get { return Path.Combine(this.SourceDevicePath, this.SourceBasePathWithDevice); }
            set { }
        }

        public KeePassDatabase KeePassDatabase
        {
            get { return this.keePassDatabase; }
            set { this.keePassDatabase = value; }
        }
        
        public void CreateBackup()
		{
            BackupSourceCollection backupSourceCollection;

            backupSourceCollection = new BackupSourceCollection(this.Configuration, this);
            backupSourceCollection.CreateBackup();
        }
    }
}
