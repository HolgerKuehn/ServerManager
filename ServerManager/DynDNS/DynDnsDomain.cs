namespace blog.dachs.ServerManager.DynDNS
{
    using blog.dachs.ServerManager;
    using System.Data;
    using System.Net;

    public class DynDnsDomain : DynDnsService
    {
        private Int32 id;
        private Int32 serviceId;
        private String name;
        private KeePassDatabase keePassDatabase;
        private String user;
        private String password;
        private DynDnsServiceCollection serviceCollection;
        private Dictionary<DynDnsIpAddressVersion, String> updateUri;

        public DynDnsDomain(Configuration configuration, Int32 dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + ID.ToString()));

            String sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsDomainProperties);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", dynDnsServiceID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            DataTable dataTable = Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            String dynDnsDomainName;
            String keePassDatabase;

            // reading values
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
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

                if (keePassDatabase != null && keePassDatabase != String.Empty)
                {
                    this.KeePassDatabase = this.KeePass.GetKeePassDatabase(keePassDatabase);

                    KeePassEntry keePassEntry = this.KeePassDatabase.GetEntry(this.Name);
                    this.User = keePassEntry.UserName;
                    this.Password = keePassEntry.Password;
                }
            }


            // reading updater URI
            UpdateUri = new Dictionary<DynDnsIpAddressVersion, String>();

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsDomain_DynDnsDomain, "reading update URIs for DynDnsDomain with DynDnsDomain_Name = " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsDomain_DynDnsDomain_ReadUpdateUri);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = Database.GetDataTable(sqlCommand);
            dataRow = null;
            DynDnsIpAddressVersion ipAddressVersion;
            String updateUri = String.Empty;

            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
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
            Int32 serviceID = 0;
            DynDnsServiceType dynDnsServiceType;
            DynDnsService dynDnsService = null;

            // iterate each Service
            for (Int32 row = 0; row < dataTable.Rows.Count; row++)
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

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }

        public Int32 ServiceID
        {
            get { return serviceId; }
            set { serviceId = value; }
        }

        public new String Name
        {
            get { return name; }
            set { name = value; }
        }

        public KeePassDatabase KeePassDatabase
        {
            get { return this.keePassDatabase; }
            set { this.keePassDatabase = value; }
        }

        public String User
        {
            get { return user; }
            set { user = value; }
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }

        public DynDnsServiceCollection ServiceCollection
        {
            get { return serviceCollection; }
            set { serviceCollection = value; }
        }

        public Dictionary<DynDnsIpAddressVersion, String> UpdateUri
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
                DynDnsIpAddress ipAddress;

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
                    ipAddress = servicePublicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "45.90.28.58";
                    servicePublicDnsServerCollection.Add(ipAddress);

                    ipAddress = servicePublicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "45.90.30.58";
                    servicePublicDnsServerCollection.Add(ipAddress);

                    ipAddress = servicePublicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "2a07:a8c0::6d:cda2";
                    servicePublicDnsServerCollection.Add(ipAddress);

                    ipAddress = servicePublicDnsServerCollection.NewIpAddress();
                    ipAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                    ipAddress.IpAddress = "2a07:a8c1::6d:cda2";
                    servicePublicDnsServerCollection.Add(ipAddress);

                    privateDnsServerCollection.Add(servicePublicDnsServerCollection);
                }

                service.IpAddressCollection.Add(privateDnsServerCollection);
                service.IpAddressCollection.WriteIpAddressCollection();
                service.SetDnsServer();
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
