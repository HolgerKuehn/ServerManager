using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blog.dachs.ServerManager
{
    public enum DataLogEntrySeverity
    {
        Debug,
        Info,
        Critical
    }

    public enum DataLogEntryOrigin
    {
        DataLogEntry_Constructor
    }

    internal class DataLogEntry
    {
        private DataLogEntrySeverity dataLogEntrySeverity = DataLogEntrySeverity.Info;
        private DataLogEntryOrigin dataLogEntryOrigin = DataLogEntryOrigin.DataLogEntry_Constructor;
        private string dataLogEntryMessage = string.Empty;

        public DataLogEntry(DataLogEntrySeverity dataLogEntrySeverity, DataLogEntryOrigin dataLogEntryOrigin, string dataLogEntryMessage)
        {
            DataLogEntrySeverity = dataLogEntrySeverity;
            DataLogEntryOrigin = dataLogEntryOrigin;
            DataLogEntryMessage = dataLogEntryMessage;
        }

        protected DataLogEntrySeverity DataLogEntrySeverity
        {
            get { return this.dataLogEntrySeverity; }
            set { this.dataLogEntrySeverity = value; }
        }

        protected DataLogEntryOrigin DataLogEntryOrigin
        {
            get { return this.dataLogEntryOrigin; }
            set { this.dataLogEntryOrigin = value; }
        }

        protected string DataLogEntryMessage
        {
            get { return this.dataLogEntryMessage; }
            set { this.dataLogEntryMessage = value; }
        }
    }
}
