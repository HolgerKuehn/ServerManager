using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsServerLocal : DynDnsServer
    {
        public DynDnsServerLocal(Configuration configuration, int dynDnsSerciceID) : base(configuration, dynDnsSerciceID)
        {
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServerLocal_DynDnsServerLocal, "set private DNS-Server IP Addresses"));

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
                    IPAddressCollection networkAdapterDnsAddresses = ipProperties.DnsAddresses;
                    
                    // DNS
                    DynDnsIpAddressCollection serverDnsAddressCollection = this.GetIpAddressCollection();
                    DynDnsIpAddress serverDnsAddress;

                    foreach (IPAddress dnsAdress in networkAdapterDnsAddresses)
                    {
                        serverDnsAddress = new DynDnsIpAddress(this.Configuration, dnsAdress.ToString());
                        serverDnsAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;

                        if (serverDnsAddress.IpAddressType != DynDnsIpAddressType.NotValid)
                        {
                            UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = ipProperties.UnicastAddresses;
                            foreach (UnicastIPAddressInformation unicastIPAddressInformation in unicastIPAddressInformationCollection)
                            {
                                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && serverDnsAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                                {
                                    serverDnsAddress.PrefixLength = (byte)unicastIPAddressInformation.PrefixLength;
                                }

                                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetworkV6 && serverDnsAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                                {
                                    serverDnsAddress.PrefixLength = (byte)unicastIPAddressInformation.PrefixLength;
                                }
                            }

                            serverDnsAddressCollection.Add(serverDnsAddress);
                        }
                    }

                    serverDnsAddressCollection.WriteIpAddressCollection();

                    // Unicast
                    // NetworkAdapter
                }
            }
        }

        public override void GetIpAddress()
        {
            // get public IP 
            base.GetPublicIpAddress();

            // get private IP 
            base.GetPrivateIpAddress();

            // invoke depending objects
            base.GetIpAddress();
        }

        public override void UpdatePublicDnsIpAddress()
        {
            foreach (DynDnsDomain domain in this.DomainCollection)
            {
                domain.UpdatePublicDnsIpAddress();
            }
        }
    }
}
