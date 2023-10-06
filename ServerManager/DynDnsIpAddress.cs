using System.Net;

namespace blog.dachs.ServerManager
{
    public enum DynDnsIpAddressType
    {
        NotValid = 1,
        Public = 2,
        Private = 3,
        LinkLocal = 4,
        UniqueLocal = 5,
        DnsServerPrivate = 6,
        DnsServerPublic = 7
    }

    public enum DynDnsIpAddressVersion
    {
        NotValid = 1,
        IPv4 = 2,
        IPv6 = 3
    }

    public class DynDnsIpAddress
    {
        private bool isValid;
        private IPAddress ipAddress;
        private IPAddress networkAddress;

        public DynDnsIpAddress()
        {
            this.IpAddress = "0";
            this.NetworkAddress = "0";
        }

        public DynDnsIpAddress(string ipAddress): base()
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
            }

        }

        public string NetworkAddress
        {
            get { return this.networkAddress.ToString(); }
            set
            {
                this.networkAddress = new IPAddress(0);
            }
        }

        public DynDnsIpAddressType DynDnsIpAddressType
        {
            get
            {
                if (!this.isValid)
                {
                    return DynDnsIpAddressType.NotValid;
                }
                else if (this.ipAddress.IsIPv6SiteLocal)
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
    }
}
