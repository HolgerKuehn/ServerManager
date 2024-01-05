namespace blog.dachs.ServerManager.DynDNS
{
    using System;
    using System.Net;
    using blog.dachs.ServerManager;

    public enum DynDnsIpAddressReferenceType : byte
    {
        DynDnsService = 1,
        DynDnsFirewallRule = 2,
        DynDnsIpAddress = 3
    }

    public enum DynDnsIpAddressType : byte
    {
        NotValid = 1,
        Public = 2,
        Private = 3,
        LinkLocal = 4,
        UniqueLocal = 5,
        DnsServerPublic = 6,
        DnsServerPrivate = 7,
        DnsServerLinkLocal = 8,
        DnsServerUniqueLocal = 9,
        Network = 10
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

        private bool isValid;
        private IPAddress ipAddress;
        private byte prefixLength;
        private IPAddress networkAddress;
        private DynDnsIpAddressType ipAddressType;

        public DynDnsIpAddress(Configuration configuration) : base(configuration)
        {
            IpAddress = "0";
        }

        public DynDnsIpAddress(Configuration configuration, string ipAddress) : this(configuration)
        {
            IpAddress = ipAddress;
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

                IsValid = false;
                ipAddress = new IPAddress(0);
                networkAddress = new IPAddress(0);

                IsValid = IPAddress.TryParse(value, out ipAddressTest);

                if (IsValid && ipAddressTest != null)
                {
                    ipAddress = ipAddressTest;
                }

                IpAddressType = DynDnsIpAddressType.NotValid;
                SetNetworkAddress();
            }

        }

        public byte PrefixLength
        {
            get { return prefixLength; }
            set
            {
                prefixLength = value;
                SetNetworkAddress();
            }
        }

        public string NetworkAddress
        {
            get { return networkAddress.ToString() + "/" + PrefixLength.ToString(); }
        }

        public DynDnsIpAddressType IpAddressType
        {
            get
            {

                if (ipAddressType == DynDnsIpAddressType.NotValid)
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
                }

                return ipAddressType;
            }

            set { ipAddressType = value; }
        }

        public DynDnsIpAddressVersion IpAddressVersion
        {
            get
            {
                if (!isValid)
                {
                    return DynDnsIpAddressVersion.NotValid;
                }
                else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return DynDnsIpAddressVersion.IPv4;
                }
                else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return DynDnsIpAddressVersion.IPv6;
                }

                return DynDnsIpAddressVersion.NotValid;
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

            if (IpAddressVersion == DynDnsIpAddressVersion.IPv4)
            {
                // set length according to protocol
                numberOfAddressBlocks = 4;
                numberOfBitsPerAddressBlocks = 8;

                // get blocks from string
                ipAddressListShort = IpAddress.Split('.').ToList();
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

        #region IComparable

        public int CompareTo(object? obj)
        {
            DynDnsIpAddress ipAddress = (DynDnsIpAddress)obj;

            if (ipAddress == null) return 0;

            if ((byte)IpAddressType < (byte)ipAddress.IpAddressType) return -1;
            else if ((byte)IpAddressType > (byte)ipAddress.IpAddressType) return +1;
            else if ((byte)IpAddressVersion < (byte)ipAddress.IpAddressVersion) return -1;
            else if ((byte)IpAddressVersion > (byte)ipAddress.IpAddressVersion) return +1;
            else return string.Compare(IpAddress, ipAddress.IpAddress, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
