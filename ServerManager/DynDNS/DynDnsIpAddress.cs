namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Data;
    using System.Net;
    using System.Net.NetworkInformation;
    using blog.dachs.ServerManager;

    public enum DynDnsIpAddressReferenceType : byte
    {
        DynDnsService = 1,
        DynDnsFirewallRule = 2,
        DynDnsIpAddress = 3
    }

    public enum DynDnsIpAddressObject : byte
    {
        NotValid = 1,
        DNSServer = 2,
        ServiceNetworkAdapter = 3,
        ServiceDNS = 4,
        UpdatedIP = 5,
        UpdatedIPResponse = 6
    }

    public enum DynDnsIpAddressType : byte
    {
        NotValid = 1,
        Public = 2,
        Private = 3,
        LinkLocal = 4,
        UniqueLocal = 5
    }

    public enum DynDnsIpAddressVersion : byte
    {
        NotValid = 1,
        IPv4 = 2,
        IPv6 = 3
    }

    public class DynDnsIpAddress : GlobalExtention, IComparable
    {
        private DynDnsIpAddressReferenceType referenceType;
        private int referenceId;
        private int ipAddressIndex;

        private DynDnsIpAddressObject ipAddressObject;
        private bool isValid;
        private IPAddress ipAddress;
        private byte prefixLength;
        private IPAddress networkAddress;
        private DynDnsIpAddressType ipAddressType;
        private DynDnsIpAddressVersion ipAddressVersion;

        public DynDnsIpAddress(Configuration configuration) : base(configuration)
        {
            this.ipAddressObject = DynDnsIpAddressObject.ServiceDNS;
            this.ipAddressVersion = DynDnsIpAddressVersion.NotValid;
            this.prefixLength = 0;

            this.IpAddress = "0";
        }

        public DynDnsIpAddress(Configuration configuration, string ipAddress) : this(configuration)
        {
            this.IpAddress = ipAddress;
        }

        public DynDnsIpAddress(Configuration configuration, int ipAddressId) : this(configuration)
        {
            DataRow dataRow;

            DynDnsIpAddressObject ipAddressObject;
            DynDnsIpAddressType ipAddressType;
            DynDnsIpAddressVersion ipAddressVersion;
            int ipAddressIndex = 0;
            string ipAddressName = string.Empty;
            string ipAddressNetworkName = string.Empty;
            string ipAddressOrganizationName = string.Empty;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsIpAddress_DynDnsIpAddress, "read IPs for ID " + ipAddressId + " from disc"));

            string sqlCommand = Database.GetCommand(Command.DynDnsIpAddress_DynDnsIpAddress);

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_DynDnsIpAddress, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressID>", Convert.ToString(ipAddressId));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_DynDnsIpAddress, sqlCommand));

            dataRow = this.Database.GetDataRow(sqlCommand, 0);

            if (dataRow != null)
            {
                ipAddressObject = (DynDnsIpAddressObject)Convert.ToByte(dataRow[0].ToString());
                ipAddressType = (DynDnsIpAddressType)Convert.ToByte(dataRow[1].ToString());
                ipAddressVersion = (DynDnsIpAddressVersion)Convert.ToByte(dataRow[2].ToString());
                ipAddressIndex = Convert.ToInt32(dataRow[3].ToString());
                ipAddressName = dataRow[4].ToString();
                ipAddressNetworkName = dataRow[5].ToString();
                ipAddressOrganizationName = dataRow[6].ToString();

                if (ipAddressName != null && ipAddressNetworkName != null)
                {
                    this.IpAddress = ipAddressName;
                    this.IpAddressObject = ipAddressObject;
                    this.IpAddressIndex = ipAddressIndex;
                    this.NetworkAddress = ipAddressNetworkName;
                }
            }
        }

        public DynDnsIpAddressReferenceType ReferenceType
        {
            get { return referenceType; }
            set { referenceType = value; }
        }

        public int ReferenceId
        {
            get { return referenceId; }
            set { referenceId = value; }
        }

        public int IpAddressIndex
        {
            get { return ipAddressIndex; }
            set { ipAddressIndex = value; }
        }

        public DynDnsIpAddressObject IpAddressObject
        {
            get { return ipAddressObject; }
            set { ipAddressObject = value; }
        }

        private bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        public string IpAddress
        {
            get { return ipAddress.ToString(); }
            set
            {
                IPAddress ipAddressTest;

                this.IsValid = false;
                this.ipAddress = new IPAddress(0);
                this.networkAddress = new IPAddress(0);

                this.IsValid = IPAddress.TryParse(value, out ipAddressTest);

                if (this.IsValid && ipAddressTest != null)
                {
                    this.ipAddress = ipAddressTest;
                }

                this.prefixLength = 0;
                this.SetIpAddressPrefix();
                this.SetNetworkAddress();
            }
        }

        public byte PrefixLength
        {
            get { return prefixLength; }
            set
            {
                prefixLength = value;
                this.SetNetworkAddress();
            }
        }

        public string NetworkAddress
        {
            get { return this.networkAddress.ToString() + "/" + this.PrefixLength.ToString(); }
            set
            {
                string networkAddressPrefixLength;
                byte ipAddressPrefixLength;

                if (value != null && value.Contains("/"))
                {
                    networkAddressPrefixLength = value.Substring(value.IndexOf("/", StringComparison.Ordinal) + 1);
                    ipAddressPrefixLength = Convert.ToByte(networkAddressPrefixLength);
                    this.PrefixLength = ipAddressPrefixLength;
                }
                
                this.SetNetworkAddress();
            }
        }

        public DynDnsIpAddressType IpAddressType
        {
            get
            {
                if (!isValid)
                {
                    ipAddressType = DynDnsIpAddressType.NotValid;
                }
                else if (ipAddress.IsIPv6LinkLocal)
                {
                    ipAddressType = DynDnsIpAddressType.LinkLocal;
                }
                else if (ipAddress.IsIPv6UniqueLocal)
                {
                    ipAddressType = DynDnsIpAddressType.UniqueLocal;
                }
                else if (
                    ipAddress.ToString().StartsWith("10.") ||
                    ipAddress.ToString().StartsWith("172.16.") ||
                    ipAddress.ToString().StartsWith("172.17.") ||
                    ipAddress.ToString().StartsWith("172.18.") ||
                    ipAddress.ToString().StartsWith("172.19.") ||
                    ipAddress.ToString().StartsWith("172.20.") ||
                    ipAddress.ToString().StartsWith("172.21.") ||
                    ipAddress.ToString().StartsWith("172.22.") ||
                    ipAddress.ToString().StartsWith("172.23.") ||
                    ipAddress.ToString().StartsWith("172.24.") ||
                    ipAddress.ToString().StartsWith("172.25.") ||
                    ipAddress.ToString().StartsWith("172.26.") ||
                    ipAddress.ToString().StartsWith("172.27.") ||
                    ipAddress.ToString().StartsWith("172.28.") ||
                    ipAddress.ToString().StartsWith("172.29.") ||
                    ipAddress.ToString().StartsWith("172.30.") ||
                    ipAddress.ToString().StartsWith("172.31.") ||
                    ipAddress.ToString().StartsWith("192.168.")
                )
                {
                    ipAddressType = DynDnsIpAddressType.Private;
                }
                else
                {
                    ipAddressType = DynDnsIpAddressType.Public;
                }

                return ipAddressType;
            }
        }

        public DynDnsIpAddressVersion IpAddressVersion
        {
            get
            {
                if (!isValid)
                {
                    this.ipAddressVersion = DynDnsIpAddressVersion.NotValid;
                }
                else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    this.ipAddressVersion = DynDnsIpAddressVersion.IPv4;
                }
                else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    this.ipAddressVersion = DynDnsIpAddressVersion.IPv6;
                }

                return this.ipAddressVersion;
            }
        }

        private void SetNetworkAddress()
        {
            List<string> networkAddressListFull = new List<string>();
            List<string> networkAddressListBit = new List<string>();
            string networkAddressPart = string.Empty;

            List<string> ipAddressListShort = new List<string>();
            List<string> ipAddressListFull = new List<string>();
            List<string> ipAddressListBit = new List<string>();
            string ipAddressPartOriginal = string.Empty;
            string ipAddressPartBit = string.Empty;

            int numberOfAddressBlocks = 0;
            int numberOfBitsPerAddressBlocks = 0;

            if (this.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
            {
                // set length according to protocol
                numberOfAddressBlocks = 4;
                numberOfBitsPerAddressBlocks = 8;

                // get blocks from string
                ipAddressListShort = this.IpAddress.Split('.').ToList();
                ipAddressListFull = ipAddressListShort;

                // convert to bit
                for (int i = 0; i < numberOfAddressBlocks; i++)
                {
                    ipAddressPartOriginal = ipAddressListFull.ElementAt(i);
                    ipAddressPartBit = Convert.ToString(Convert.ToByte(ipAddressPartOriginal), 2);
                    ipAddressPartBit = ipAddressPartBit.PadLeft(numberOfBitsPerAddressBlocks, '0');
                    ipAddressListBit.Add(ipAddressPartBit);
                }
            }

            if (IpAddressVersion == DynDnsIpAddressVersion.IPv6)
            {
                // set length according to protocol
                numberOfAddressBlocks = 8;
                numberOfBitsPerAddressBlocks = 16;

                // get blocks from string
                ipAddressListShort = IpAddress.Split(':').ToList();

                // add padding for short notation
                for (int addressBlock = 0; addressBlock < ipAddressListShort.Count; addressBlock++)
                {
                    int addressBlockOffset = 0;
                    int numberOfEmptyBlocks = 8 - ipAddressListShort.Count;

                    ipAddressPartOriginal = ipAddressListShort.ElementAt(addressBlock);

                    // add empty block until all 8 blocks are present
                    if (ipAddressPartOriginal.Length == 0)
                    {
                        for (addressBlockOffset = 0; addressBlockOffset < numberOfEmptyBlocks; addressBlockOffset++)
                        {
                            ipAddressListFull.Add("0000");
                        }
                    }

                    // add leading 0 to existing block
                    ipAddressPartOriginal = ipAddressPartOriginal.PadLeft(numberOfBitsPerAddressBlocks, '0');

                    // add block to FullAddress
                    ipAddressListFull.Add(ipAddressPartOriginal);
                }

                // convert to bit
                for (int i = 0; i < numberOfAddressBlocks; i++)
                {
                    ipAddressPartOriginal = ipAddressListFull.ElementAt(i);
                    ipAddressPartBit = Convert.ToString(Convert.ToInt16(ipAddressPartOriginal, 16), 2);
                    ipAddressPartBit = ipAddressPartBit.PadLeft(numberOfBitsPerAddressBlocks, '0');
                    ipAddressListBit.Add(ipAddressPartBit);
                }
            }


            // apply networkAddress
            for (int addressBlock = 0; addressBlock < numberOfAddressBlocks; addressBlock++)
            {
                networkAddressPart = string.Empty;
                ipAddressPartBit = ipAddressListBit.ElementAt(addressBlock);

                for (int bit = 0; bit < numberOfBitsPerAddressBlocks; bit++)
                {
                    if (ipAddressPartBit.ElementAt(bit) == '1' && addressBlock * numberOfBitsPerAddressBlocks + bit < PrefixLength)
                    {
                        networkAddressPart += "1";
                    }
                    else
                    {
                        networkAddressPart += "0";
                    }
                }

                networkAddressListBit.Add(networkAddressPart);
            }


            // convert to byte or hex notation
            string networkAddress = string.Empty;

            for (int addressBlock = 0; addressBlock < numberOfAddressBlocks; addressBlock++)
            {
                networkAddressPart = networkAddressListBit.ElementAt(addressBlock);


                if (IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                {
                    networkAddressPart = Convert.ToString(Convert.ToByte(networkAddressPart, 2));
                    networkAddress += networkAddressPart.ToLower();

                    if (addressBlock < 3)
                        networkAddress += '.';
                }


                if (IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                {
                    networkAddressPart = Convert.ToInt16(networkAddressPart, 2).ToString("X4");
                    networkAddress += networkAddressPart.ToLower();

                    if (addressBlock < 7)
                        networkAddress += ':';
                }
            }

            IPAddress ipAddressTest;
            IPAddress.TryParse(networkAddress, out ipAddressTest);

            if (ipAddressTest != null)
            {
                this.networkAddress = ipAddressTest;
            }
        }

        public void SetIpAddressPrefix()
        {
            DynDnsIpAddressCollection dnsServerCollection;
            dnsServerCollection = new DynDnsIpAddressCollection(this.Configuration);
            dnsServerCollection.ReferenceType = this.ReferenceType;
            dnsServerCollection.ReferenceId = this.ReferenceId;

            byte prefixLength = 0;
            
            if (this.PrefixLength == 0)
            {
                if (prefixLength == 0 && this.IpAddressType == DynDnsIpAddressType.LinkLocal)
                {
                    dnsServerCollection.Clear();
                    dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.LinkLocal);
                    if (dnsServerCollection.Count > 0) prefixLength = dnsServerCollection.ElementAt(0).PrefixLength;
                }

                if (prefixLength == 0 && this.IpAddressType == DynDnsIpAddressType.UniqueLocal)
                {
                    dnsServerCollection.Clear();
                    dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.UniqueLocal);
                    if (dnsServerCollection.Count > 0) prefixLength = dnsServerCollection.ElementAt(0).PrefixLength;
                }

                if (prefixLength == 0 && this.IpAddressType == DynDnsIpAddressType.Private)
                {
                    dnsServerCollection.Clear();
                    dnsServerCollection.ReadIpAddressCollection(DynDnsIpAddressObject.DNSServer, DynDnsIpAddressType.Private);
                    if (dnsServerCollection.Count > 0) prefixLength = dnsServerCollection.ElementAt(0).PrefixLength;
                }


                if (prefixLength == 0 && this.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                {
                    prefixLength = 32;
                }

                if (prefixLength == 0 && this.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                {
                    prefixLength = 64;
                }

                this.PrefixLength = prefixLength;
            }
        }

        /// <summary>
        /// prepares a dataset to hold an IPAddress
        /// </summary>
        public void PrepareIpAddressToDisc()
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsIpAddress_PrepareIpAddressToDisc, "prepare Database for IP (IP Address)"));

            string sqlCommand = this.Database.GetCommand(Command.DynDnsIpAddress_PerpareIpAddressToDisc_IpAddress);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_PrepareIpAddressToDisc, sqlCommand));

            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)this.ReferenceType));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.ReferenceId));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressObjectID>", Convert.ToString((byte)this.IpAddressObject));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)this.IpAddressVersion));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)this.IpAddressType));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressIndex>", Convert.ToString((byte)this.IpAddressIndex));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_PrepareIpAddressToDisc, sqlCommand));

            this.Database.ExecuteCommand(sqlCommand);
        }

        /// <summary>
        /// Writes IpAddress to disc
        /// </summary>
        public void WriteIpAddress()
        {
            this.PrepareIpAddressToDisc();

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsIpAddress_WriteIpAddress, "write IP to disc"));

            string sqlCommandReadIpAddressID = this.Database.GetCommand(Command.DynDnsIpAddress_WriteIpAddress_ReadIpAddressID);
            string sqlCommandWriteIpAddress = this.Database.GetCommand(Command.DynDnsIpAddress_WriteIpAddress_WriteIpAddress);

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_WriteIpAddress, sqlCommandReadIpAddressID));
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_WriteIpAddress, sqlCommandWriteIpAddress));

            // read primary key of IP address
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)this.ReferenceType));
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.ReferenceId));
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressObjectID>", Convert.ToString((byte)this.IpAddressObject));
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)this.IpAddressVersion));
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)this.IpAddressType));
            sqlCommandReadIpAddressID = sqlCommandReadIpAddressID.Replace("<DynDnsIpAddressIndex>", Convert.ToString(this.IpAddressIndex));

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_WriteIpAddress, sqlCommandReadIpAddressID));
    
            DataRow dataRow = this.Database.GetDataRow(sqlCommandReadIpAddressID, 0);
            int ipAddressAddressID = 0;
    
            if (dataRow != null)
            {
                ipAddressAddressID = Convert.ToInt32(dataRow[0].ToString());
            }
    
            if (ipAddressAddressID != 0)
            {
                // set IP-Address
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressID>", ipAddressAddressID.ToString());
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressReferenceTypeID>", Convert.ToString((byte)this.ReferenceType));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressReferenceID>", Convert.ToString(this.ReferenceId));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressObjectID>", Convert.ToString((byte)this.IpAddressObject));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressVersionID>", Convert.ToString((byte)this.IpAddressVersion));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressTypeID>", Convert.ToString((byte)this.IpAddressType));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressIndex>", Convert.ToString(this.IpAddressIndex));
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressAddress>", this.IpAddress);
                sqlCommandWriteIpAddress = sqlCommandWriteIpAddress.Replace("<DynDnsIpAddressNetwork>", this.NetworkAddress);
    
                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsIpAddress_WriteIpAddress, sqlCommandWriteIpAddress));
                this.Database.ExecuteCommand(sqlCommandWriteIpAddress);
            }
        }
        
        #region IComparable

        public int CompareTo(object? obj)
        {
            DynDnsIpAddress ipAddress = (DynDnsIpAddress)obj;

            if (ipAddress == null) return 0;

            if ((byte)IpAddressObject < (byte)ipAddress.IpAddressObject) return -1;
            else if ((byte)IpAddressObject > (byte)ipAddress.IpAddressObject) return +1;
            else if ((byte)IpAddressType < (byte)ipAddress.IpAddressType) return -1;
            else if ((byte)IpAddressType > (byte)ipAddress.IpAddressType) return +1;
            else if ((byte)IpAddressVersion < (byte)ipAddress.IpAddressVersion) return -1;
            else if ((byte)IpAddressVersion > (byte)ipAddress.IpAddressVersion) return +1;
            else return string.Compare(IpAddress, ipAddress.IpAddress, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
