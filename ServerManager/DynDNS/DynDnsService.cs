using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
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
            IpAddressCollection = new DynDnsIpAddressCollection(Configuration);
            IpAddressCollection.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_DynDnsService, "reading DynDnsService properties"));

            DynDnsServiceID = dynDnsServiceID;
            Name = string.Empty;

            string sqlCommand = Database.GetCommand(Command.DynDnsService_DynDnsService);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", DynDnsServiceID.ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_DynDnsService, sqlCommand));

            DataTable dataTable = Database.GetDataTable(sqlCommand);
            DataRow dataRow = null;
            string dynDnsServiceName = string.Empty;

            // iterate each Domain
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsServiceName = dataRow[0].ToString();

                if (dynDnsServiceName != null)
                {
                    Name = dynDnsServiceName;
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsService_DynDnsService, "created DynDnsService with DynDnsService_Name = " + Name));
                    ReadIpAddressCollectionFromDisc();
                }
            }
        }

        public int DynDnsServiceID
        {
            get { return dynDnsServiceID; }
            set
            {
                dynDnsServiceID = value;
                IpAddressCollection.ReferenceId = value;
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DynDnsIpAddressCollection IpAddressCollection
        {
            get { return ipAddressCollection; }
            set { ipAddressCollection = value; }
        }

        public virtual void GetIpAddress()
        {
        }

        public virtual void SetDnsServer()
        {
        }

        public virtual void GetDnsIpAddress(DynDnsIpAddressCollection dnsIpAddressCollection)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetDnsIpAddress, "request DNS IP for " + Name));

            string powerShellCommand = Database.GetCommand(Command.DynDnsService_GetDnsIpAddress);
            powerShellCommand = powerShellCommand.Replace("<DomainName>", name);

            if (Socket.OSSupportsIPv6)
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv6).ElementAt(0).IpAddress.ToString());
            }
            else
            {
                powerShellCommand = powerShellCommand.Replace("<DnsServer>", dnsIpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv4).ElementAt(0).IpAddress.ToString());
            }

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_GetDnsIpAddress, powerShellCommand));

            ProcessOutput ipAddressList = PowerShell.ExecuteCommand(powerShellCommand);

            int i = 4;
            while (true)
            {
                string ipAddress = CommandLine.GetProcessOutput(ipAddressList, i);

                if (ipAddress != null)
                {
                    IpAddressCollection.Add(ipAddress.Trim());
                }
                else
                {
                    break;
                }

                i++;
            }

            CommandLine.DeleteProcessOutput(ipAddressList);
            SetIpAddressPrefix();
        }

        public virtual void GetPublicIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_GetPublicIpAddress, "request Public IP  from DNS for " + Name));

            IpAddressCollection.Remove(DynDnsIpAddressType.Public);
            GetDnsIpAddress(IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic));

            SetIpAddressPrefix();
        }

        public void SetIpAddressPrefix()
        {
            byte prefixLength;
            foreach (DynDnsIpAddress dynDnsIpAddress in IpAddressCollection)
            {
                if (dynDnsIpAddress.PrefixLength == 0)
                {
                    if (
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.UniqueLocal ||
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.LinkLocal
                    )
                    {
                        prefixLength = IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerUniqueLocal).ElementAt(0).PrefixLength;
                    }
                    else if (
                        dynDnsIpAddress.IpAddressType == DynDnsIpAddressType.Private
                    )
                    {
                        prefixLength = IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPrivate).ElementAt(0).PrefixLength;
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

            IpAddressCollection.Remove(DynDnsIpAddressType.NotValid);
            WriteIpAddressCollectionToDisc();
        }

        private void ReadIpAddressCollectionFromDisc()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_ReadIpAddressCollectionFromDisc, "read IPs for Service " + Name + " from disc"));
            IpAddressCollection.ReadIpAddressCollectionFromDisc();

            // check for DnsServer and add default if not presemt
            DynDnsIpAddressCollection ipAddressDnsServer = ipAddressCollection.GetIpAddressCollection(DynDnsIpAddressType.DnsServerPublic);

            if (ipAddressDnsServer.Count == 0)
            {
                Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_ReadIpAddressCollectionFromDisc, "set Public DNS-Server for " + Name));

                DynDnsIpAddress dynDnsIpAddress;

                dynDnsIpAddress = new DynDnsIpAddress(Configuration, "45.90.28.58");
                dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
                dynDnsIpAddress.PrefixLength = 32;
                IpAddressCollection.Add(dynDnsIpAddress);

                dynDnsIpAddress = new DynDnsIpAddress(Configuration, "2a07:a8c0::6d:cda2");
                dynDnsIpAddress.IpAddressType = DynDnsIpAddressType.DnsServerPublic;
                dynDnsIpAddress.PrefixLength = 64;
                IpAddressCollection.Add(dynDnsIpAddress);
            }
        }

        private void PrepareIpAddressCollectionToDisc(DynDnsIpAddressCollection ipAddressCollection)
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-SetSourceFileCollection from " + Name + " (IP Address)"));

            string sqlCommandOriginal = Database.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_IpAddress);
            string sqlCommandReplace;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                sqlCommandReplace = sqlCommandOriginal;
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)DynDnsIpAddressReferenceType.DynDnsService));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(DynDnsServiceID));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReplace = sqlCommandReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandReplace));

                Database.ExecuteCommand(sqlCommandReplace);
            }

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, "prepare Database for IP-SetSourceFileCollection from " + Name + " (Network Address)"));

            string sqlCommandNetwork = Database.GetCommand(Command.DynDnsService_PerpareIpAddressCollectionToDisc_NetworkAddress);
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_PrepareIpAddressCollectionToDisc, sqlCommandNetwork));
            Database.ExecuteCommand(sqlCommandNetwork);
        }

        public void WriteIpAddressCollectionToDisc()
        {
            WriteIpAddressCollectionToDisc(Command.DynDnsService_WriteIpAddressCollectionToDisc_ReadIpAddressID, IpAddressCollection);
        }

        public void WriteIpAddressCollectionToDisc(Command command, DynDnsIpAddressCollection ipAddressCollection)
        {
            PrepareIpAddressCollectionToDisc(ipAddressCollection);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, "write IP-SetSourceFileCollection from " + Name + " to disc"));

            string sqlCommandReadIpAddressIDOriginal = Database.GetCommand(command);
            string sqlCommandReadIpAddressIDReplace;
            string sqlCommandWriteIpAddressOriginal = Database.GetCommand(Command.DynDnsService_WriteIpAddressCollectionToDisc_WriteIpAddress);
            string sqlCommandWriteIpAddressReplace;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDOriginal));
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressOriginal));

            foreach (DynDnsIpAddress dynDnsIpAddress in ipAddressCollection)
            {
                // read primary key of IP address
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDOriginal;
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<ConfigurationID>", Convert.ToString(Configuration.ConfigurationID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(DynDnsServiceID));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressType));
                sqlCommandReadIpAddressIDReplace = sqlCommandReadIpAddressIDReplace.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)dynDnsIpAddress.IpAddressVersion));

                Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandReadIpAddressIDReplace));

                DataTable dataTable = Database.GetDataTable(sqlCommandReadIpAddressIDReplace);
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

                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    Database.ExecuteCommand(sqlCommandWriteIpAddressReplace);

                    // set Network-Address
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressOriginal;
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressName>", dynDnsIpAddress.NetworkAddress.ToString());
                    sqlCommandWriteIpAddressReplace = sqlCommandWriteIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressNetworkID);

                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, sqlCommandWriteIpAddressReplace));
                    Database.ExecuteCommand(sqlCommandWriteIpAddressReplace);
                }
            }
        }

        public virtual void WriteLogForChangedIpAddress()
        {
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, "write changed IPs for " + Name + " to log"));

            string sqlCommandWriteLogForChangedIpAddress = Database.GetCommand(Command.DynDnsService_WriteLogForChangedIpAddress);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, sqlCommandWriteLogForChangedIpAddress));

            sqlCommandWriteLogForChangedIpAddress = sqlCommandWriteLogForChangedIpAddress.Replace("<DynDnsServiceID>", Convert.ToString(DynDnsServiceID));

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, sqlCommandWriteLogForChangedIpAddress));

            DataTable dataTable = Database.GetDataTable(sqlCommandWriteLogForChangedIpAddress);
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

                Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsService_WriteLogForChangedIpAddress, "Service \"" + Name + "\" changend " + ipAddressTypeID.ToString() + " IP on " + ipAddressVersion + " to " + ipAddress));
            }
        }

        public virtual void UpdatePublicDnsIpAddress()
        {
            UpdatePublicDnsIpAddress(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadIpAddressIDNonPublicIp, "set non-public IPs from " + Name + " as updated");
        }

        public virtual void UpdatePublicDnsIpAddress(Command command, string logMessage)
        {
            string dynDnsIpAddressIpAddressID = string.Empty;
            string dynDnsIpAddressNetworkID = string.Empty;
            DataTable dataTable;
            DataRow dataRow;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsService_WriteIpAddressCollectionToDisc, logMessage));

            string sqlCommandReadIpAddressIDCommand = Database.GetCommand(command);


            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandReadIpAddressIDCommand));

            sqlCommandReadIpAddressIDCommand = sqlCommandReadIpAddressIDCommand.Replace("<DynDnsIpAddressReferenceID>", DynDnsServiceID.ToString());
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandReadIpAddressIDCommand));

            dataTable = Database.GetDataTable(sqlCommandReadIpAddressIDCommand);
            dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsIpAddressIpAddressID = dataRow[0].ToString();
                dynDnsIpAddressNetworkID = dataRow[1].ToString();

                // IP Address
                if (dynDnsIpAddressIpAddressID != null)
                    UpdatePublicDnsIpAddress(dynDnsIpAddressIpAddressID);


                // Network Address
                if (dynDnsIpAddressNetworkID != null)
                    UpdatePublicDnsIpAddress(dynDnsIpAddressNetworkID);
            }
        }

        public virtual void UpdatePublicDnsIpAddress(string dynDnsIpAddressId)
        {
            string sqlCommandUpdateIpAddressOriginal = Database.GetCommand(Command.DynDnsService_UpdatePublicDnsIpAddress_UpdateIpAddress);
            string sqlCommandUpdateIpAddressReplace = string.Empty;

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandUpdateIpAddressOriginal));

            sqlCommandUpdateIpAddressReplace = sqlCommandUpdateIpAddressOriginal;
            sqlCommandUpdateIpAddressReplace = sqlCommandUpdateIpAddressReplace.Replace("<DynDnsIpAddressID>", dynDnsIpAddressId);
            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsService_UpdatePublicDnsIpAddress, sqlCommandUpdateIpAddressReplace));
            Database.ExecuteCommand(sqlCommandUpdateIpAddressReplace);
        }

        public virtual void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
        }
    }
}
