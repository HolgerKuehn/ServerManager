namespace blog.dachs.ServerManager
{
    using System.Collections.Generic;
    using System.Data;
    using System.Net;
    using System.Net.Sockets;

    public enum DynDnsServiceType : byte
    {
        Server = 1,
        ServiceLocal = 2,
        ServiceRemote = 3,
        Domain = 4
    }

    public abstract class DynDnsService : GlobalExtention
    {
        private int dynDnsServiceID;
        private string name;
        private DynDnsIpAddressCollection ipAddressCollection;

        public DynDnsService(Configuration configuration, int dynDnsServiceID) : base(configuration)
        {
            this.IpAddressCollection = new DynDnsIpAddressCollection(this.Configuration);
            this.IpAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading DynDnsService properties"));

            this.DynDnsServiceID = dynDnsServiceID;
            this.Name = string.Empty;

            string sqlCommand = this.Database.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = this.Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsServiceName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceName = dataRow[0].ToString();
                
                if (dynDnsServiceName != null)
                {
                    this.Name = dynDnsServiceName;
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsService_DynDnsService, "created DynDnsService with DynDnsService_Name = " + this.Name));
                    this.ReadIpAddressCollectionFromDisc();
                }
            }
        }

        public int DynDnsServiceID
        {
            get { return this.dynDnsServiceID; }
            set
            {
                this.dynDnsServiceID = value;
                this.IpAddressCollection.ReferenceId = value;
            }
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

            string powerShellCommand = this.Database.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", this.name);

            if (Socket.OSSupportsIPv6)
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv6).ElementAt(0).IpAddress.ToString());
            } 
            else
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv4).ElementAt(0).IpAddress.ToString());
            }

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_GetDnsIpAddress, powerShellCommand));

            List<string> ipAddressList = this.PowerShell.Command(powerShellCommand);

            foreach (string ipAddress in ipAddressList)
            {
                this.IpAddressCollection.Add(ipAddress.Trim());
            }

            this.SetIpAddressPrefix();
        }

        public virtual void GetPublicIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP  from DNS for " + this.Name));

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
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_ReadIpAddressCollectionFromDisc, "read IPs for Service " + this.Name + " from disc"));
            this.IpAddressCollection.ReadIpAddressCollectionFromDisc();

            // check for DnsServer and add default if not presemt
            DynDnsIpAddressCollection ipAddressDnsServer = this.ipAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic);
            
            if (ipAddressDnsServer.Count == 0)
            {
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_ReadIpAddressCollectionFromDisc, "set Public DNS-Server for " + this.Name));

                DynDnsIpAddress dynDnsIpAddress;

                dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "45.90.28.58");
                dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
                dynDnsIpAddress.PrefixLength = 32;
                this.IpAddressCollection.Add(dynDnsIpAddress);

                dynDnsIpAddress = new DynDnsIpAddress(this.Configuration, "2a07:a8c0::6d:cda2");
                dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
                dynDnsIpAddress.PrefixLength = 64;
                this.IpAddressCollection.Add(dynDnsIpAddress);
            }
        }

        private void PrepareIpAddressCollectionToDisc(DynDnsIpAddressCollection ipAddressCollection)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-SourceFileCollection from " + this.Name + " (IP Address)"));

            string sqlCommandOriginal = this.Database.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_IpAddress);
            string sqlCommandReplace;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                sqlCommandReplace = sqlCommandOriginal;
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)DynDnsIpAddressReferenceType.DynDnsService));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.DynDnsServiceID));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandReplace));

                this.Database.ExecuteCommand(sqlCommandReplace);
            }

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-SourceFileCollection from " + this.Name + " (Network Address)"));

            string sqlCommandNetwork = this.Database.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_NetworkAddress);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandNetwork));
            this.Database.ExecuteCommand(sqlCommandNetwork);
        }

        public void WriteIpAddressCollectionToDisc()
        {
            WriteIpAddressCollectionToDisc(Command.DynDnsService_WriteIpAddressCollectionToDisc_ReadIpAddressID, this.IpAddressCollection);
        }

        public void WriteIpAddressCollectionToDisc(Command command, DynDnsIpAddressCollection ipAddressCollection)
        {
            this.PrepareIpAddressCollectionToDisc(ipAddressCollection);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, "write IP-SourceFileCollection from " + this.Name + " to disc"));

            string sqlCommandReadIpAddressIDOriginal = this.Database.GetCommand(command);
            string sqlCommandReadIpAddressIDReplace;
            string sqlCommandWriteIpAddressOriginal = this.Database.GetCommand(Command.DynDnsService_WriteIpAddressCollectionToDisc_WriteIpAddress);
            string sqlCommandWriteIpAddressReplace;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDOriginal));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                // read primary key of IP address
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDOriginal;
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<ConfigurationID>", Convert.ToString(this.Configuration.ConfigurationID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.DynDnsServiceID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDReplace));

                DataTable dataTable = this.Database.GetDataTable(sqlCommandReadIpAddressIDReplace);
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
                
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    this.Database.ExecuteCommand(sqlCommandWriteIpAddressReplace);

                    // set Network-Address
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressOriginal;
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressName>", dynDnsIpAddress.NetworkAddress.ToString());
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressNetworkID);

                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    this.Database.ExecuteCommand(sqlCommandWriteIpAddressReplace);
                }
            }
        }

        public virtual void WriteLogForChangedIpAddress()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, "write changed IPs for " + this.Name + " to log"));

            string sqlCommandWriteLogForChangedIpAddress = this.Database.GetCommand(Command.DynDnsService_WriteLogForChangedIpAddress);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, sqlCommandWriteLogForChangedIpAddress));

            sqlCommandWriteLogForChangedIpAddress = sqlCommandWriteLogForChangedIpAddress.Replace("<DynDnsServiceID>", Convert.ToString(this.DynDnsServiceID));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, sqlCommandWriteLogForChangedIpAddress));

            DataTable dataTable = this.Database.GetDataTable(sqlCommandWriteLogForChangedIpAddress);
            DataRow dataRow = null;
            DynDnsIpAddressType ipAddressTypeID = 0;
            DynDnsIpAddressVersion ipAddressVersion = 0;
            string ipAddress = string.Empty;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                ipAddressTypeID = (DynDnsIpAddressType)Convert.ToByte(dataRow[0].ToString());
                ipAddressVersion = (DynDnsIpAddressVersion)Convert.ToByte(dataRow[1].ToString());
                ipAddress = dataRow[2].ToString();

                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, "Service \"" + this.Name + "\" changend " + ipAddressTypeID.ToString() + " IP on " + ipAddressVersion + " to " + ipAddress));
            }
        }

        public virtual void UpdatePublicDnsIpAddress()
        {
            this.UpdatePublicDnsIpAddress(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadIpAddressIDNonPublicIp, "set non-public IPs from " + this.Name + " as updated");
        }

        public virtual void UpdatePublicDnsIpAddress(Command command, string logMessage)
        {
            string dynDnsIpAddressIpAddressID = string.Empty;
            string dynDnsIpAddressNetworkID = string.Empty;
            DataTable dataTable;
            DataRow dataRow;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, logMessage));

            string sqlCommandReadIpAddressIDCommand = this.Database.GetCommand(command);


            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandReadIpAddressIDCommand));

            sqlCommandReadIpAddressIDCommand = sqlCommandReadIpAddressIDCommand.Replace("<DynDnsIpAddressReferenceID>", this.DynDnsServiceID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandReadIpAddressIDCommand));

            dataTable = this.Database.GetDataTable(sqlCommandReadIpAddressIDCommand);
            dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsIpAddressIpAddressID = dataRow[0].ToString();
                dynDnsIpAddressNetworkID = dataRow[1].ToString();

                // IP Address
                if (dynDnsIpAddressIpAddressID != null)
                    this.UpdatePublicDnsIpAddress(dynDnsIpAddressIpAddressID);


                // Network Address
                if (dynDnsIpAddressNetworkID != null)
                    this.UpdatePublicDnsIpAddress(dynDnsIpAddressNetworkID);
            }
        }

        public virtual void UpdatePublicDnsIpAddress(string dynDnsIpAddressId)
        {
            string sqlCommandUpdateIpAddressOriginal = this.Database.GetCommand(Command.DynDnsService_UpdatePublicDnsIpAddress_UpdateIpAddress);
            string sqlCommandUpdateIpAddressReplace = string.Empty;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandUpdateIpAddressOriginal));

            sqlCommandUpdateIpAddressReplace = sqlCommandUpdateIpAddressOriginal;
            sqlCommandUpdateIpAddressReplace = sqlCommandUpdateIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressId);
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandUpdateIpAddressReplace));
            this.Database.ExecuteCommand(sqlCommandUpdateIpAddressReplace);
        }

        public virtual void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
        }
    }
}
