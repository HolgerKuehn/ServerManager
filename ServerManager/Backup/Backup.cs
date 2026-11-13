namespace blog.dachs.ServerManager.Backup
{
    using System.Data;

    public class Backup : GlobalExtention
	{ 
		private Int32 backupId;
		private String name;
        private String sourceDevicePath;
        private String sourceBasePath;
        private Int32 sourceNameDepth;
        private BackupDestinationDevice destinationDevice;
		private String destinationBasePath;
        private KeePassDatabase keePassDatabase;

        public Backup(Configuration configuration, Int32 backupId) : base(configuration)
        {
			this.backupId = backupId;

            // read set properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.Backup_Backup, "reading Backup properties"));

            String sqlCommand = this.Database.GetCommand(Command.Backup_Backup);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.backupId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.Backup_Backup, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
			DataRow dataRow = null;
            String name;
            String sourceDevicePath;
            String sourceBasePath;
            Int32 sourceNameDepth;
            Int32 destinationDeviceId;
            String destinationBasePath;
            String keePassDatabase;

            // get Backup properties 
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
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

        public Int32 BackupId
		{
			get { return this.backupId; }
			set { this.backupId = value; }
		}

        public String Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

        public String SourceDevicePath
        {
            get { return this.sourceDevicePath; }
            set { this.sourceDevicePath = value; }
        }

        public String SourceBasePathWithDevice
		{
			get { return Path.Combine(this.SourceDevicePath, this.sourceBasePath); }
			set { this.sourceBasePath = value; }
		}

        public String SourceBasePathWithoutDevice
        {
            get { return this.sourceBasePath; }
            set { this.sourceBasePath = value; }
        }

        public Int32 SourceNameDepth
        {
            get { return sourceNameDepth; }
            set { this.sourceNameDepth = value; }
        }

        public BackupDestinationDevice DestinationDevice
        {
            get { return this.destinationDevice; }
            set { this.destinationDevice = value; }
        }

        public String DestinationDevicePath
		{
			get { return this.DestinationDevice.Path; }
			set { this.DestinationDevice.Path = value; }
		}

        public String DestinationBasePath
		{
			get { return this.destinationBasePath; }
			set { this.destinationBasePath = value; }
		}

        public String FullAbsolutePath
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
