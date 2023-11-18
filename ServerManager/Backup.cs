using System.Data;

namespace blog.dachs.ServerManager
{
	public class Backup : GlobalExtention
	{ 
		private int backupId;
		private string name;
		private string sourceBasePath;
        private int sourceNameDepth;
        private string destinationDevicePath;
		private string destinationBasePath;
        private BackupSourceCollection backupSourceCollection;

        public Backup(Configuration configuration, int backupId) : base(configuration)
        {
			this.backupId = backupId;

            // read backupSource properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.Backup_Backup, "reading Backup properties"));

            string sqlCommand = this.Database.GetCommand(Command.Backup_Backup);
            sqlCommand = sqlCommand.Replace("<BackupID>", this.backupId.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.Backup_Backup, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
			DataRow dataRow = null;
            string name;
            string sourceBasePath;
            int sourceNameDepth;
            string destinationDevicePath;
            string destinationBasePath;

            // get BackupID 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                name = dataRow[0].ToString();
                sourceBasePath = dataRow[1].ToString();
                sourceNameDepth = Convert.ToInt32(dataRow[2].ToString());
                destinationDevicePath = dataRow[3].ToString();
                destinationBasePath = dataRow[4].ToString();

				if (name != null)
					this.Name = name;
				
				if (sourceBasePath != null)
					this.SourceBasePath = sourceBasePath;

                if (destinationDevicePath != null)
                    this.DestinationDevicePath = destinationDevicePath;

                this.SourceNameDepth = sourceNameDepth;

                if (destinationBasePath != null)
                    this.DestinationBasePath = destinationBasePath;
            }

			this.BackupSourceCollection = new BackupSourceCollection(this.Configuration, this);
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

        public int SourceNameDepth
        {
            get { return sourceNameDepth; }
            set { this.sourceNameDepth = value; }
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

        public BackupSourceCollection BackupSourceCollection
        {
            get { return this.backupSourceCollection; }
            set { this.backupSourceCollection = value; }
        }

        public void ReadFromFilesystem()
		{
			this.BackupSourceCollection.ReadFromFilesystem();
        }

        public void WriteToDisc()
        {
            this.BackupSourceCollection.WriteToDisc();
        }
    }
}
