using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace blog.dachs.ServerManager
{
    public class DynDnsDomain : DynDnsService
    {
        private int id;
        private string name;
        private string user;
        private string password;
        private DynDnsServiceCollection serviceCollection;
        private Dictionary<DynDnsIpAddressVersion, string> updateUri;


        public DynDnsDomain(Configuration configuration, int dynDnsDomainID, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            this.ID = dynDnsDomainID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + this.ID.ToString()));
            
            string sqlCommand = this.Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsDomainProperties);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
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
                    this.Name = dynDnsDomainName;
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "created DynDnsDomain with DynDnsDomain_Name = " + this.Name));
                }

                if (dynDnsDomainUserBase64 != null)
                {
                    this.User = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainUserBase64));
                }

                if (dynDnsDomainPasswordBase64 != null)
                {
                    this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainPasswordBase64));
                }
            }

            // reading updater URI
            this.UpdateUri = new Dictionary<DynDnsIpAddressVersion, string>();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "reading update URIs for DynDnsDomain with DynDnsDomain_Name = " + this.Name));

            sqlCommand = this.Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_ReadUpdateUri);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = this.Database.GetDataTable(sqlCommand);
            dataRow = null;
            DynDnsIpAddressVersion ipAddressVersion;
            string updateUri = string.Empty;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                ipAddressVersion = (DynDnsIpAddressVersion)Convert.ToByte(dataRow[0].ToString());
                updateUri = dataRow[1].ToString();

                if(updateUri != null)
                    this.UpdateUri.Add(ipAddressVersion, updateUri);
            }

            // reading servies
            this.ServiceCollection = new DynDnsServiceCollection(this.Configuration);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "reading services for DynDnsDomain with DynDnsDomain_Name = " + this.Name));

            sqlCommand = this.Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsServices);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = this.Database.GetDataTable(sqlCommand);
            dataRow = null;
            int serviceID = 0;
            DynDnsServiceType dynDnsServiceType;
            DynDnsService dynDnsService = null;

            // iterate each Service
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                serviceID = Convert.ToInt32(dataRow[0].ToString());
                dynDnsServiceType = (DynDnsServiceType)Convert.ToByte(dataRow[1].ToString());

                // create Service accordingly to dynDnsServiceTypeID
                switch (dynDnsServiceType)
                {
                    case DynDnsServiceType.ServiceLocal:
                        dynDnsService = new DynDnsServiceLocal(this.Configuration, serviceID);
                        break;

                    case DynDnsServiceType.ServiceRemote:
                        dynDnsService = new DynDnsServiceRemote(this.Configuration, serviceID);
                        break;
                }

                this.ServiceCollection.Add(dynDnsService);
            }
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public new string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string User
        {
            get { return this.user; }
            set { this.user = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public DynDnsServiceCollection ServiceCollection
        {
            get { return serviceCollection; }
            set { this.serviceCollection = value; }
        }

        public Dictionary<DynDnsIpAddressVersion, string> UpdateUri
        {
            get { return this.updateUri; }
            set { this.updateUri = value; }
        }

        public override void GetIpAddress()
        {
            foreach (DynDnsService service in this.ServiceCollection)
            {
                service.GetIpAddress();
            }
        }

        public override void SetDnsServer()
        {
            foreach (DynDnsService service in this.ServiceCollection)
            {
                DynDnsIpAddressCollection ipAddressCollection = service.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic);
                if (ipAddressCollection.Count == 0)
                {
                    service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));
                }

                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));

                service.SetDnsServer();
            }
        }

        public override void WriteLogForChangedIpAddress()
        {
            foreach (DynDnsService service in this.ServiceCollection)
            {
                service.WriteLogForChangedIpAddress();
            }
        }

        public override void UpdatePublicDnsIpAddress()
        {
            NetworkCredential networkCredential = new NetworkCredential(this.User, this.Password);

            foreach (DynDnsService service in this.ServiceCollection)
            {
                service.UpdatePublicDnsIpAddress();

                for (byte ipAddressVersion = 2; ipAddressVersion < 4; ipAddressVersion++)
                {
                    if (this.UpdateUri.ContainsKey((DynDnsIpAddressVersion)ipAddressVersion))
                    {
                        service.UpdatePublicDnsIpAddress(this.UpdateUri[(DynDnsIpAddressVersion)ipAddressVersion], networkCredential, (DynDnsIpAddressVersion)ipAddressVersion);
                    }
                }
            }
        }
    }
}
