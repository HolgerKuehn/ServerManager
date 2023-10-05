namespace blog.dachs.ServerManager
{
    public class DataLogEntry
    {
        private DataLogSeverity dataLogSeverity = DataLogSeverity.Informational;
        private DataLogOrigin dataLogOrigin     = DataLogOrigin.ProgramMain_Main;
        private string dataLogMessage           = string.Empty;

        public DataLogEntry(DataLogSeverity dataLogSeverity, DataLogOrigin dataLogOrigin, string dataLogMessage)
        {
            DataLogSeverity = dataLogSeverity;
            DataLogOrigin   = dataLogOrigin;
            DataLogMessage  = dataLogMessage;
        }

        public DataLogSeverity DataLogSeverity
        {
            get { return this.dataLogSeverity; }
            set { this.dataLogSeverity = value; }
        }

        public DataLogOrigin DataLogOrigin
        {
            get { return this.dataLogOrigin; }
            set { this.dataLogOrigin = value; }
        }

        public string DataLogMessage
        {
            get { return this.dataLogMessage; }
            set { this.dataLogMessage = value; }
        }

        public string DataLogInsert()
        { 
            string sqlInsertHeader = string.Empty;
            string sqlInsertValue = string.Empty;

            sqlInsertHeader += "insert into DataLog ";
            sqlInsertHeader += "   (";
            sqlInsertValue  += "select ";

            sqlInsertHeader +=                "DataLog_Timestamp, ";
            sqlInsertValue  += "unixepoch() as DataLog_Timestamp, ";

            sqlInsertHeader +=                                              "DataLogSeverity_ID, ";
            sqlInsertValue  += ((int)this.DataLogSeverity).ToString() + " as DataLogSeverity_ID, ";

            sqlInsertHeader +=                                            "DataLogOrigin_ID, ";
            sqlInsertValue  += ((int)this.DataLogOrigin).ToString() + " as DataLogOrigin_ID, ";

            sqlInsertHeader +=                                    "DataLog_Message";
            sqlInsertValue  += "\"" + this.DataLogMessage + "\" as DataLog_Message";

            sqlInsertHeader += ") ";

            return sqlInsertHeader + sqlInsertValue;
        }
    }
}
