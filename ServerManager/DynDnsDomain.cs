using System.Data;
using System.Text;

namespace blog.dachs.ServerManager
{
    public class DynDnsDomain : DynDnsService
    {
        private int id;
        private string name;
        private string user;
        private string password;

        private DynDnsServiceCollection serviceCollection;

        public DynDnsDomain(Configuration configuration, int dynDnsDomainID, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            this.ID = dynDnsDomainID;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "creating DynDnsDomain with DynDnsDomain_ID = " + this.ID.ToString()));
            
            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsDomainProperties);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

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

            // reading servies
            this.ServiceCollection = new DynDnsServiceCollection(this.Configuration);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, "reading services for DynDnsDomain with DynDnsDomain_Name = " + this.Name));

            sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsDomain_DynDnsDomain_DynDnsServices);
            sqlCommand = sqlCommand.Replace("<DynDnsDomainID>", this.ID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsDomain_DynDnsDomain, sqlCommand));

            dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
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

        public string Name
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
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate));
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerLinkLocal));
                service.IpAddressCollection.Add(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal));

                service.SetDnsServer();
            }
        }
    }
}
