namespace blog.dachs.ServerManager
{
    using Microsoft.VisualBasic;
    using System.Collections.Generic;
    using System.Data;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public enum DynDnsServiceType : byte
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

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading network object properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsServiceName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceName = dataRow[0].ToString();
                
                if (dynDnsServiceName != null)
                    this.Name = dynDnsServiceName;
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

        public virtual void GetIpAddress()
        {
        }

        public virtual void SetDnsServer()
        {
        }
        
        public virtual void GetDnsIpAddress(DynDnsIpAddressCollection dnsIpAddressCollection)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + this.Name));

            string powerShellCommand = this.HandlerDatabase.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.name);

            if (Socket.OSSupportsIPv6)
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv6).ElementAt(0).IpAddress.ToString());
            } 
            else
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv4).ElementAt(0).IpAddress.ToString());
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
            this.GetDnsIpAddress(this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));

            this.SetIpAddressPrefix();
        }

        public void SetIpAddressPrefix()
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

        private void PrepareIpAddressCollectionToDisc(DynDnsIpAddressCollection ipAddressCollection)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-Colection from " + this.Name + " (IP Address)"));

            string sqlCommandOriginal = this.HandlerDatabase.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_IpAddress);
            string sqlCommandReplace;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                sqlCommandReplace = sqlCommandOriginal;
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)DynDnsIpAddressReferenceType.DynDnsService));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.DynDnsServiceID));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandReplace));

                this.HandlerDatabase.ExecuteCommand(sqlCommandReplace);
            }

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-Colection from " + this.Name + " (Network Address)"));

            string sqlCommandNetwork = this.HandlerDatabase.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_NetworkAddress);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandNetwork));
            this.HandlerDatabase.ExecuteCommand(sqlCommandNetwork);
        }

        public void WriteIpAddressCollectionToDisc()
        {
            WriteIpAddressCollectionToDisc(Command.DynDnsService_WriteIpAddressCollectionToDisc_ReadIpAddressID, this.IpAddressCollection);
        }

        public void WriteIpAddressCollectionToDisc(Command command, DynDnsIpAddressCollection ipAddressCollection)
        {
            this.PrepareIpAddressCollectionToDisc(ipAddressCollection);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, "write IP-Colection from " + this.Name + " to disc"));

            string sqlCommandReadIpAddressIDOriginal = this.HandlerDatabase.GetCommand(command);
            string sqlCommandReadIpAddressIDReplace;
            string sqlCommandWriteIpAddressOriginal = this.HandlerDatabase.GetCommand(Command.DynDnsService_WriteIpAddressCollectionToDisc_WriteIpAddress);
            string sqlCommandWriteIpAddressReplace;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDOriginal));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                // read primary key of IP address
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDOriginal;
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<ConfigurationID>", Convert.ToString(this.Configuration.ConfigurationID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.DynDnsServiceID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDReplace));

                DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommandReadIpAddressIDReplace);
                DataRow dataRow = null;
                string dynDnsIpAddressIpAddressID = string.Empty;
                string dynDnsIpAddressNetworkID = string.Empty;

                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    dataRow = dataTable.Rows[row];
                    dynDnsIpAddressIpAddressID = dataRow[0].ToString();
                    dynDnsIpAddressNetworkID = dataRow[1].ToString();
                }

                if (dynDnsIpAddressIpAddressID != string.Empty)
                { 
                    // set IP-Address
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressOriginal;
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressName>", dynDnsIpAddress.IpAddress.ToString());
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressIpAddressID);
                
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    this.HandlerDatabase.ExecuteCommand(sqlCommandWriteIpAddressReplace);

                    // set Network-Address
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressOriginal;
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressName>", dynDnsIpAddress.NetworkAddress.ToString());
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressNetworkID);

                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    this.HandlerDatabase.ExecuteCommand(sqlCommandWriteIpAddressReplace);
                }
            }
        }

        public void UpdatePublicDnsIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, "Update public IP from " + this.Name));

            string sqlCommandNonPublicIpAddressType = this.HandlerDatabase.GetCommand(Command.DynDnsService_UpdatePublicDnsIpAddress_NonPublicIpAddressType);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandNonPublicIpAddressType));
            this.HandlerDatabase.ExecuteCommand(sqlCommandNonPublicIpAddressType);


            string sqlCommandReadRemoteServiceID = this.HandlerDatabase.GetCommand(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadRemoteServiceID);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandReadRemoteServiceID));
            
            string sqlCommandNonRemoteServiceOriginal = this.HandlerDatabase.GetCommand(Command.DynDnsService_UpdatePublicDnsIpAddress_NonRemoteService);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandNonRemoteServiceOriginal));
            string sqlCommandNonRemoteServiceReplace;

            DataTable dataTable = this.HandlerDatabase.GetDataTable(sqlCommandReadRemoteServiceID);
            DataRow dataRow = null;
            string dynDnsIpAddressIpAddressID = string.Empty;
            string dynDnsIpAddressNetworkID = string.Empty;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsIpAddressIpAddressID = dataRow[0].ToString();
                dynDnsIpAddressNetworkID = dataRow[1].ToString();

                // IP Address
                sqlCommandNonRemoteServiceReplace = sqlCommandNonRemoteServiceOriginal;
                sqlCommandNonRemoteServiceReplace = sqlCommandNonRemoteServiceReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressIpAddressID);
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandNonRemoteServiceReplace));
                this.HandlerDatabase.ExecuteCommand(sqlCommandNonRemoteServiceReplace);


                // Network Address
                sqlCommandNonRemoteServiceReplace = sqlCommandNonRemoteServiceOriginal;
                sqlCommandNonRemoteServiceReplace = sqlCommandNonRemoteServiceReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressNetworkID);
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandNonRemoteServiceReplace));
                this.HandlerDatabase.ExecuteCommand(sqlCommandNonRemoteServiceReplace);
            }
        }
    }
}
