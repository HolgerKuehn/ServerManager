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
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        DynDnsIpAddress networkIPAddress = new DynDnsIpAddress(dnsAdress.ToString());
                        if (networkIPAddress.IsValid)
                        {
                            this.SetNetworkDnsServer(networkIPAddress.DynDnsIpAddressVersion, networkIPAddress);
                        }
                    }
                }
            }
        }
    }
}
