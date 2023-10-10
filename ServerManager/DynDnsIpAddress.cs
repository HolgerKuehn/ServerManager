namespace blog.dachs.ServerManager
{
    using System;
    using System.Net;

    public enum DynDnsIpAddressType
    {
        NotValid = 1,
        Public = 2,
        Private = 3,
        LinkLocal = 4,
        UniqueLocal = 5,
        DnsServerPrivate = 6,
        DnsServerPublic = 7,
        Network = 8
    }

    public enum DynDnsIpAddressVersion
    {
        NotValid = 1,
        IPv4 = 2,
        IPv6 = 3
    }

    public class DynDnsIpAddress : GlobalExtention
    {
        private bool isValid;
        private IPAddress ipAddress;
        private byte prefixLength;
        private IPAddress networkAddress;

        public DynDnsIpAddress(Configuration configuration) : base(configuration)
        {
            this.IpAddress = "0";
        }

        public DynDnsIpAddress(Configuration configuration, string ipAddress) : this(configuration)
        {
            this.IpAddress = ipAddress;
        }

        public bool IsValid
        {
            get { return this.isValid; }
            set { isValid = value; }
        }

        public string IpAddress
        {
            get { return this.ipAddress.ToString(); }
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

                this.SetNetworkAddress();
            }

        }

        public byte PrefixLength
        {
            get { return this.prefixLength; }
            set 
            {
                this.prefixLength = value;
                this.SetNetworkAddress();
            }
        }

        public string NetworkAddress
        {
            get { return this.networkAddress.ToString() + "/" + this.PrefixLength.ToString(); }
        }

        public DynDnsIpAddressType DynDnsIpAddressType
        {
            get
            {
                if (!this.isValid)
                {
                    return DynDnsIpAddressType.NotValid;
                }
                else if (this.ipAddress.IsIPv6LinkLocal)
                {
                    return DynDnsIpAddressType.LinkLocal;
                }
                else if (this.ipAddress.IsIPv6UniqueLocal)
                {
                    return DynDnsIpAddressType.UniqueLocal;
                }
                else if (
                    this.ipAddress.ToString().StartsWith("10.") ||
                    this.ipAddress.ToString().StartsWith("172.16.") ||
                    this.ipAddress.ToString().StartsWith("172.17.") ||
                    this.ipAddress.ToString().StartsWith("172.18.") ||
                    this.ipAddress.ToString().StartsWith("172.19.") ||
                    this.ipAddress.ToString().StartsWith("172.20.") ||
                    this.ipAddress.ToString().StartsWith("172.21.") ||
                    this.ipAddress.ToString().StartsWith("172.22.") ||
                    this.ipAddress.ToString().StartsWith("172.23.") ||
                    this.ipAddress.ToString().StartsWith("172.24.") ||
                    this.ipAddress.ToString().StartsWith("172.25.") ||
                    this.ipAddress.ToString().StartsWith("172.26.") ||
                    this.ipAddress.ToString().StartsWith("172.27.") ||
                    this.ipAddress.ToString().StartsWith("172.28.") ||
                    this.ipAddress.ToString().StartsWith("172.29.") ||
                    this.ipAddress.ToString().StartsWith("172.30.") ||
                    this.ipAddress.ToString().StartsWith("172.31.") ||
                    this.ipAddress.ToString().StartsWith("192.168.")
                ) {
                    return DynDnsIpAddressType.Private;
                }

                return DynDnsIpAddressType.Public;
            }
        }

        public DynDnsIpAddressVersion DynDnsIpAddressVersion
        {
            get
            {
                if (!this.isValid)
                {
                    return DynDnsIpAddressVersion.NotValid;
                }
                else if (this.ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return DynDnsIpAddressVersion.IPv4;
                }
                else if (this.ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
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

            if (this.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv4)
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

            if (this.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv6)
            {
                // set length according to protocol
                numberOfAddressBlocks = 8;
                numberOfBitsPerAddressBlocks = 16;

                // get blocks from string
                ipAddressListShort = this.IpAddress.Split(':').ToList();

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
                    if (ipAddressPartBit.ElementAt(bit) == '1' && addressBlock * numberOfBitsPerAddressBlocks + bit < this.PrefixLength)
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


                if (this.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv4)
                {
                    networkAddressPart = Convert.ToString(Convert.ToByte(networkAddressPart, 2));
                    networkAddress += networkAddressPart.ToLower();

                    if (addressBlock < 3)
                        networkAddress += '.';
                }


                if (this.DynDnsIpAddressVersion == DynDnsIpAddressVersion.IPv6)
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
    }
}
