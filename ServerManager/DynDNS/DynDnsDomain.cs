using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    using System.Data;
    using System.Net;
    using System.Text;

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
            ID = dynDnsDomainID;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + ID.ToString()));

            string sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsDomainProperties);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", ID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            DataTable dataTable = Database.GetDataTable(sqlCommand);
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
                    Name = dynDnsDomainName;
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "created DynDnsDomain with DynDnsDomain_Name = " + Name));
                }

                if (dynDnsDomainUserBase64 != null)
                {
                    User = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainUserBase64));
                }

                if (dynDnsDomainPasswordBase64 != null)
                {
                    Password = Encoding.UTF8.GetString(Convert.FromBase64String(dynDnsDomainPasswordBase64));
                }
            }

            // reading updater URI
            UpdateUri = new Dictionary<DynDnsIpAddressVersion, string>();

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "reading update URIs for DynDnsDomain with DynDnsDomain_Name = " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_ReadUpdateUri);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", ID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = Database.GetDataTable(sqlCommand);
            dataRow = null;
            DynDnsIpAddressVersion ipAddressVersion;
            string updateUri = string.Empty;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                ipAddressVersion = (DynDnsIpAddressVersion)Convert.ToByte(dataRow[0].ToString());
                updateUri = dataRow[1].ToString();

                if (updateUri != null)
                    UpdateUri.Add(ipAddressVersion, updateUri);
            }

            // reading servies
            ServiceCollection = new DynDnsServiceCollection(Configuration);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "reading services for DynDnsDomain with DynDnsDomain_Name = " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsServices);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", ID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = Database.GetDataTable(sqlCommand);
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
                        dynDnsService = new DynDnsServiceLocal(Configuration, serviceID);
                        break;

                    case DynDnsServiceType.ServiceRemote:
                        dynDnsService = new DynDnsServiceRemote(Configuration, serviceID);
                        break;
                }

                ServiceCollection.Add(dynDnsService);
            }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public new string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public DynDnsServiceCollection ServiceCollection
        {
            get { return serviceCollection; }
            set { serviceCollection = value; }
        }

        public Dictionary<DynDnsIpAddressVersion, string> UpdateUri
        {
            get { return updateUri; }
            set { updateUri = value; }
        }

        public override void GetIpAddress()
        {
            foreach (DynDnsService service in ServiceCollection)
            {
                service.GetIpAddress();
            }
        }

        public override void SetDnsServer()
        {
            foreach (DynDnsService service in ServiceCollection)
            {
                DynDnsIpAddressCollection ipAddressCollection = service.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic);
                if (ipAddressCollection.Count == 0)
                {
                    service.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));
                }

                service.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
                service.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
                service.IpAddressCollection.Add(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));

                service.SetDnsServer();
            }
        }

        public override void WriteLogForChangedIpAddress()
        {
            foreach (DynDnsService service in ServiceCollection)
            {
                service.WriteLogForChangedIpAddress();
            }
        }

        public override void UpdatePublicDnsIpAddress()
        {
            NetworkCredential networkCredential = new NetworkCredential(User, Password);

            foreach (DynDnsService service in ServiceCollection)
            {
                service.UpdatePublicDnsIpAddress();

                for (byte ipAddressVersion = 2; ipAddressVersion < 4; ipAddressVersion++)
                {
                    if (UpdateUri.ContainsKey((DynDnsIpAddressVersion)ipAddressVersion))
                    {
                        service.UpdatePublicDnsIpAddress(UpdateUri[(DynDnsIpAddressVersion)ipAddressVersion], networkCredential, (DynDnsIpAddressVersion)ipAddressVersion);
                    }
                }
            }
        }
    }
}
