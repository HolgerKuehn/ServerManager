using System.Data;
using System.Text;

namespace blog.dachs.ServerManager
{
    public class DynDnsDomain : GlobalExtention
    {
        private int dynDnsDomainID;
        private string dynDnsDomainName;
        private string dynDnsDomainUser;
        private string dynDnsDomainPassword;

        public DynDnsDomain(Configuration configuration, int dynDnsDomainID) : base(configuration)
        {
            this.DynDnsDomainID = dynDnsDomainID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + this.DynDnsDomainID.ToString()));
            
            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsDomain_DynDnsDomain);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.DynDnsDomainID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsDomainName;
            string dynDnsDomainUserBase64;
            string dynDnsDomainPasswordBase64;

            // reading values
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsDomainName = dataRow[0].ToString();
                dynDnsDomainUserBase64 = dataRow[1].ToString();
                dynDnsDomainPasswordBase64 = dataRow[2].ToString();

                if (dynDnsDomainName != null)
                {
                    this.DynDnsDomainName = dynDnsDomainName;
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "created DynDnsDomain with DynDnsDomain_Name = " + this.DynDnsDomainName));
                }

                if (dynDnsDomainUserBase64 != null)
                {
                    this.DynDnsDomainUser = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainUserBase64));
                }

                if (dynDnsDomainPasswordBase64 != null)
                {
                    this.DynDnsDomainPassword = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainPasswordBase64));
                }
            }

            // reading updater URI

            // reading servies


        }

        public int DynDnsDomainID
        {
            get { return this.dynDnsDomainID; }
            set { this.dynDnsDomainID = value; }
        }

        public string DynDnsDomainName
        {
            get { return this.dynDnsDomainName; }
            set { this.dynDnsDomainName = value; }
        }

        public string DynDnsDomainUser
        {
            get { return this.dynDnsDomainUser; }
            set { this.dynDnsDomainUser = value; }
        }

        public string DynDnsDomainPassword
        {
            get { return this.dynDnsDomainPassword; }
            set { this.dynDnsDomainPassword = value; }
        }
    }
}
