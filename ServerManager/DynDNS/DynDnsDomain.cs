namespace blog.dachs.ServerManager.DynDNS
{
    using blog.dachs.ServerManager;
    using System.Data;
    using System.Net;

    public class DynDnsDomain : DynDnsService
    {
        private int id;
        private int serviceId;
        private string name;
        private KeePassDatabase keePassDatabase;
        private string user;
        private string password;
        private DynDnsServiceCollection serviceCollection;
        private Dictionary<DynDnsIpAddressVersion, string> updateUri;

        public DynDnsDomain(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + ID.ToString()));

            string sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsDomainProperties);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", dynDnsServiceID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            DataTable dataTable = Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsDomainName;
            string keePassDatabase;

            // reading values
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                this.ID = Convert.ToInt32(dataRow[0].ToString());
                dynDnsDomainName = dataRow[1].ToString();
                keePassDatabase = dataRow[2].ToString();

                if (dynDnsDomainName != null)
                {
                    this.Name = dynDnsDomainName;
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "created DynDnsDomain with DynDnsDomain_Name = " + Name));
                }

                if (keePassDatabase != null && keePassDatabase != string.Empty)
                {
                    this.KeePassDatabase = this.KeePass.GetKeePassDatabase(keePassDatabase);

                    KeePassEntry keePassEntry = this.KeePassDatabase.GetEntry(this.Name);
                    this.User = keePassEntry.UserName;
                    this.Password = keePassEntry.Password;
                }
            }


            // reading updater URI
            UpdateUri = new Dictionary<DynDnsIpAddressVersion, string>();

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "reading update URIs for DynDnsDomain with DynDnsDomain_Name = " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_ReadUpdateUri);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

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

            // reading services
            this.ServiceCollection = new DynDnsServiceCollection(Configuration);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "reading services for DynDnsDomain with DynDnsDomain_Name = " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsServices);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", ID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

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

                if (dynDnsService != null)
                {
                    dynDnsService.Parent = this;
                    this.ServiceCollection.Add(dynDnsService);
                }
            }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public int ServiceID
        {
            get { return serviceId; }
            set { serviceId = value; }
        }

        public new string Name
        {
            get { return name; }
            set { name = value; }
        }

        public KeePassDatabase KeePassDatabase
        {
            get { return this.keePassDatabase; }
            set { this.keePassDatabase = value; }
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
            foreach (DynDnsService service in this.ServiceCollection)
            {
                service.GetIpAddress();
            }
        }

        public override void SetDnsServer()
        {
            foreach (DynDnsService service in this.ServiceCollection)
            {
                DynDnsIpAddressCollection domainPublicDnsServerCollection = this.NewIpAddressCollection();
                DynDnsIpAddressCollection servicePublicDnsServerCollection = service.NewIpAddressCollection();
                DynDnsIpAddressCollection privateDnsServerCollection = this.NewIpAddressCollection();

                List<DynDnsIpAddressType> dynDnsIpAddressTypes = [
                    DynDnsIpAddressType.Private,
                    DynDnsIpAddressType.LinkLocal,
                    DynDnsIpAddressType.UniqueLocal,
                ];

                domainPublicDnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public);
                servicePublicDnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Public);
                privateDnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, dynDnsIpAddressTypes);

                servicePublicDnsServerCollection.Remove(DynDnsIpAddressType.NotValid);
                privateDnsServerCollection.Remove(DynDnsIpAddressType.NotValid);


                if (servicePublicDnsServerCollection.Count == 0)
                {
                    service.IpAddressCollection.Add(domainPublicDnsServerCollection);
                }

                service.IpAddressCollection.Add(privateDnsServerCollection);

                service.SetDnsServer();
                service.IpAddressCollection.WriteIpAddressCollection();
            }
        }

        public override void UpdatePublicDnsIpAddress()
        {
            NetworkCredential networkCredential = new NetworkCredential(this.User, this.Password);

            foreach (DynDnsService service in this.ServiceCollection)
            {
                // create IP-Address for Updated IP and Updated IP Response
                service.UpdatePublicDnsIpAddress();

                // update changed IPs
                for (byte ipAddressVersion = 2; ipAddressVersion <= 3; ipAddressVersion++)
                {
                    if (this.UpdateUri.ContainsKey((DynDnsIpAddressVersion)ipAddressVersion))
                    {
                        service.UpdatePublicDnsIpAddress(UpdateUri[(DynDnsIpAddressVersion)ipAddressVersion], networkCredential, (DynDnsIpAddressVersion)ipAddressVersion);
                    }
                }
            }
        }

        public override void WriteIpAddressHistory()
        {
            foreach (DynDnsService service in this.ServiceCollection)
            {
                service.WriteIpAddressHistory();
            }
        }
    }
}
