using System.Data;

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
		private BackupFileCollection backupFileCollection;

        public Backup(Configuration configuration, int backupId) : base(configuration)
        {
			this.backupId = backupId;

            // read backup properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.Backup_Backup, "reading Backup properties"));

            string sqlCommand = this.Database.GetCommand(Command.Backup_Backup);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.backupId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.Backup_Backup, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
			DataRow dataRow = null;
            string name;
            string sourceBasePath;
            string destinationDevicePath;
            string destinationBasePath;
			string keePassFile;

            // get BackupID 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                name = dataRow[0].ToString();
                sourceBasePath = dataRow[1].ToString();
                destinationDevicePath = dataRow[2].ToString();
                destinationBasePath = dataRow[3].ToString();
                keePassFile = dataRow[4].ToString();

				if (name != null)
					this.Name = name;
				
				if (sourceBasePath != null)
					this.SourceBasePath = sourceBasePath;

                if (destinationDevicePath != null)
                    this.DestinationDevicePath = destinationDevicePath;

                if (destinationBasePath != null)
                    this.DestinationBasePath = destinationBasePath;

                if (keePassFile != null)
                    this.KeePassFile = keePassFile;
            }

			this.backupFileCollection = new BackupFileCollection(this.Configuration, this);
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

        public BackupFileCollection BackupFileCollection
        {
            get { return this.backupFileCollection; }
            set { this.backupFileCollection = value; }
        }

        public void ReadFileList()
		{
			this.BackupFileCollection.ReadFromFilesystem();
            // FindDeletedFiles();
        }

        public void WriteChangedToDisc()
        {
            this.BackupFileCollection.WriteChangedToDisc();
        }
    }
}
