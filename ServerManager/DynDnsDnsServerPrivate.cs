using System.Net.NetworkInformation;
using System.Net;

namespace blog.dachs.ServerManager
{
    public class DynDnsDnsServerPrivate : DynDnsDnsServer
    {
        public DynDnsDnsServerPrivate()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (
                    networkInterface.OperationalStatus == OperationalStatus.Up && 
                    (
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    ) &&
                    !networkInterface.Description.Contains("Bluetooth")
                )
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        DynDnsIpAddress networkIPAddress = new DynDnsIpAddress(dnsAdress.ToString());
                        if (networkIPAddress.IsValid)
                        {
                            this.SetDynDnsDnsServer(networkIPAddress.DynDnsIpAddressVersion, networkIPAddress);
                        }
                    }

                    UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = ipProperties.UnicastAddresses;
                    foreach (UnicastIPAddressInformation unicastIPAddressInformation in unicastIPAddressInformationCollection)
                    {
                        if (unicastIPAddressInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            this.SetDynDnsDnsServerIpAddressPrefixLength(DynDnsIpAddressVersion.IPv4, (byte)unicastIPAddressInformation.PrefixLength);
                        }

                        if (unicastIPAddressInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            this.SetDynDnsDnsServerIpAddressPrefixLength(DynDnsIpAddressVersion.IPv6, (byte)unicastIPAddressInformation.PrefixLength);
                        }
                    }
                }
            }
        }
    }
}
