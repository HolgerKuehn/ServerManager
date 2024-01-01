using System.Data;
using System.Security.Cryptography;

namespace blog.dachs.ServerManager.Backup
{
    public class BackupDestinationDevice : GlobalExtention
    {
        private int id;
        private string path;
        private ulong availableSize;
        private ulong usedSize;
        private ulong freeSize;

        public BackupDestinationDevice(Configuration configuration, int destinationDeviceId) : base(configuration)
        {
            this.Id = destinationDeviceId;

            // read properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupDestinationDevice_BackupDestinationDevice, "reading BackupDestinationDevice properties"));

            string sqlCommand = this.Database.GetCommand(Command.BackupDestinationDevice_BackupDestinationDevice);
            sqlCommand = sqlCommand.Replace("<BackupDestinationDeviceID>", this.Id.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupDestinationDevice_BackupDestinationDevice, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string path;

            // get Backup properties 
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                path = dataRow[0].ToString();
                this.AvailableSize = Convert.ToUInt64(dataRow[1].ToString());
                this.UsedSize = Convert.ToUInt64(dataRow[2].ToString());
                this.FreeSize = Convert.ToUInt64(dataRow[3].ToString());

                if (path != null)
                    this.Path = path;
            }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public ulong AvailableSize
        {
            get { return this.availableSize; }
            set
            {
                this.availableSize = value;

                this.freeSize = this.AvailableSize - this.UsedSize;
            }
        }

        public ulong UsedSize
        {
            get { return this.usedSize; }
            set
            { 
                this.usedSize = value;

                this.freeSize = this.AvailableSize - this.UsedSize;
            }
        }

        public ulong FreeSize
        {
            get { return this.freeSize; }
            set { }
        }

        public void WriteToDisc()
        {
            // read properties from disk
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.BackupDestinationDevice_WriteToDisc, "writing BackupDestinationDevice properties to disc"));

            string sqlCommand = this.Database.GetCommand(Command.BackupDestinationDevice_WriteToDisc);
            sqlCommand = sqlCommand.Replace("<BackupDestinationDeviceID>", this.Id.ToString());
            sqlCommand = sqlCommand.Replace("<BackupDestinationDeviceAvailableSize>", this.AvailableSize.ToString());
            sqlCommand = sqlCommand.Replace("<BackupDestinationDevice_UsedSize>", this.UsedSize.ToString());
            sqlCommand = sqlCommand.Replace("<BackupDestinationDeviceFreeSize>", this.FreeSize.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.BackupDestinationDevice_WriteToDisc, sqlCommand));
            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
