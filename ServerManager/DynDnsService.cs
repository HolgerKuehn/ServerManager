namespace blog.dachs.ServerManager
{
    using System.Collections.Generic;
    using System.Data;
    using System.Net;
    using System.Net.Sockets;

    public enum DynDnsServiceType
    {
        Server = 1,
        ServiceLocal = 2,
        ServiceRemote = 3
    }

    public abstract class DynDnsService : GlobalExtention
    {
        private int dynDnsServiceID;
        private string name;
        private DynDnsIpAddressCollection ipAddressCollection;

        public DynDnsService(Configuration configuration, int dynDnsServiceID) : base(configuration)
        {
            this.IpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            this.ReadIpAddressCollectionFromDisc();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "set public DNS-Server IP Addresses"));
            
            DynDnsIpAddress dynDnsIpAddress;

            dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "45.90.28.58");
            dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
            dynDnsIpAddress.PrefixLength = 32;
            this.IpAddressCollection.Add(dynDnsIpAddress);

            dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "2a07:a8c0::6d:cda2");
            dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
            dynDnsIpAddress.PrefixLength = 64;
            this.IpAddressCollection.Add(dynDnsIpAddress);


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading network object properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string DynDnsServiceName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                DynDnsServiceName = dataRow[0].ToString();
                
                if (DynDnsServiceName != null)
                    this.Name = DynDnsServiceName;
            }
        }

        public int DynDnsServiceID
        {
            get { return this.dynDnsServiceID; }
            set { this.dynDnsServiceID = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public DynDnsIpAddressCollection IpAddressCollection
        {
            get { return ipAddressCollection; }
            set { this.ipAddressCollection = value; }
        }

        public virtual void GetDnsIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommand = this.HandlerDatabase.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.name);

            if (Socket.OSSupportsIPv6)
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal).ElementAt(0).IpAddress.ToString());
            } 
            else
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate).ElementAt(0).IpAddress.ToString());
            }
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, powerShellCommand));

            List<string> ipAddressList = this.HandlerPowerShell.Command(powerShellCommand);

            foreach (string ipAddress in ipAddressList)
            {
                this.IpAddressCollection.Add(ipAddress.Trim());
            }

            this.SetIpAddressPrefix();
        }

        public virtual void GetPublicIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP for " + this.Name));

            this.IpAddressCollection.Remove(DynDnsIpAddressType.Public);

            string publicIP = string.Empty;

            if (Socket.OSSupportsIPv4)
            {
                publicIP = this.HandlerWebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv4);
                this.IpAddressCollection.Add(publicIP.Trim());
            }

            if (Socket.OSSupportsIPv6)
            {
                publicIP = this.HandlerWebRequest.Request("https://ident.me", DynDnsIpAddressVersion.IPv6);
                this.IpAddressCollection.Add(publicIP.Trim());
            }

            this.SetIpAddressPrefix();
        }

        private void SetIpAddressPrefix()
        {
            byte prefixLength;
            foreach (DynDnsIpAddress dynDnsIpAddress in this.IpAddressCollection)
            {
                if (dynDnsIpAddress.PrefixLength == 0)
                {
                    if (
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.UniqueLocal ||
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.LinkLocal
                    )
                    {
                        prefixLength = this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal).ElementAt(0).PrefixLength;
                    }
                    else if (
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.Private
                    )
                    {
                        prefixLength = this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate).ElementAt(0).PrefixLength;
                    }
                    else if (dynDnsIpAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                    {
                        prefixLength = 32;
                    }
                    else
                    {
                        prefixLength = 64;
                    }

                    dynDnsIpAddress.PrefixLength = prefixLength;
                }
            }

            this.IpAddressCollection.Remove(DynDnsIpAddressType.NotValid);
            this.WriteIpAddressCollectionToDisc();
        }

        private void ReadIpAddressCollectionFromDisc()
        {

        }

        private void PerpareIpAddressCollectionToDisc()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PerpareIpAddressCollectionToDisc, "prepare Database for IP-Colection from " + this.Name + " (IP Address)"));

            string sqlCommandOriginal = this.HandlerDatabase.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_IpAddress);
            string sqlCommandReplace;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PerpareIpAddressCollectionToDisc, sqlCommandOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in this.IpAddressCollection)
            {
                sqlCommandReplace = sqlCommandOriginal;
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)DynDnsIpAddressReferenceType.DynDnsService));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.DynDnsServiceID));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PerpareIpAddressCollectionToDisc, sqlCommandReplace));

                this.HandlerDatabase.ExecuteCommand(sqlCommandReplace);
            }

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PerpareIpAddressCollectionToDisc, "prepare Database for IP-Colection from " + this.Name + " (Network Address)"));

            string sqlCommandNetwork = this.HandlerDatabase.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_NetworkAddress);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PerpareIpAddressCollectionToDisc, sqlCommandNetwork));
            this.HandlerDatabase.ExecuteCommand(sqlCommandNetwork);
        }

        private void WriteIpAddressCollectionToDisc()
        {
            this.PerpareIpAddressCollectionToDisc();


        }
    }
}
