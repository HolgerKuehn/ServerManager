namespace blog.dachs.ServerManager
{
    using System.Data;

    public abstract class DynDnsNetworkObject : GlobalExtention
    {
        private int dynDnsServiceID;
        private string name;

        public DynDnsNetworkObject(Configuration configuration, int dynDnsServiceID) : base(configuration)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_DynDnsNetworkObject, "reading network object properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsNetworkObject_DynDnsNetworkObject);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_DynDnsNetworkObject, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsNetworkObjectName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsNetworkObjectName = dataRow[0].ToString();
                
                if (dynDnsNetworkObjectName != null)
                    this.Name = dynDnsNetworkObjectName;
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int DynDnsServiceID
        {
            get { return this.dynDnsServiceID; }
            set { this.dynDnsServiceID = value; }
        }

        public virtual DynDnsIpAddressCollection GetIpAddress(DynDnsDnsServer dynDnsDnsServer)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_GetIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommand = this.HandlerDatabase.GetCommand(Command.DynDnsNetworkObject_GetIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.name);
            powerShellCommand = powerShellCommand.Replace("<DnsServer>", dynDnsDnsServer.GetDynDnsDnsServerIp().IpAddress);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsNetworkObject_GetIpAddress, powerShellCommand));

            List<string> ipAddressList = this.HandlerPowerShell.Command(powerShellCommand);

            DynDnsIpAddressCollection dynDnsIpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);

            foreach (string ipAddress in ipAddressList)
            {
                DynDnsIpAddress dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, ipAddress.Trim());

                if (dynDnsIpAddress.IsValid)
                {
                    dynDnsIpAddressCollection.Add(dynDnsIpAddress);
                }
                  
            }

            return dynDnsIpAddressCollection;
        }
    }
}
