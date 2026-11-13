using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace blog.dachs.ServerManager.DynDNS
{
    public class DynDnsServerLocal : DynDnsServer
    {
        public DynDnsServerLocal(Configuration configuration, Int32 dynDnsSerciceID) : base(configuration, dynDnsSerciceID)
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
                    !networkInterface.Description.Contains("Bluetooth") &&
                    !networkInterface.Name.Contains("Default Switch")
                )
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection networkAdapterDnsAddresses = ipProperties.DnsAddresses;
                    
                    // DNS
                    DynDnsIpAddressCollection serverDnsAddressCollection = this.NewIpAddressCollection();
                    DynDnsIpAddress serverDnsAddress;

                    foreach (IPAddress dnsAdress in networkAdapterDnsAddresses)
                    {
                        serverDnsAddress = serverDnsAddressCollection.NewIpAddress();
                        serverDnsAddress.IpAddressObject = DynDnsIpAddressObject.DNSServer;
                        serverDnsAddress.IpAddress = dnsAdress.ToString();

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
                }
            }
        }

        public override void GetIpAddress()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            DynDnsIpAddressCollection serverAddressCollection = this.NewIpAddressCollection();
            DynDnsIpAddress serverIpAddress;

            // IPv4
            serverAddressCollection.Add(this.GetPublicIpAddress());

            // IPv6 and local addresses
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (
                    networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    ) &&
                    !networkInterface.Description.Contains("Bluetooth") &&
                    !networkInterface.Name.Contains("Default Switch")
                )
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    
                    UnicastIPAddressInformationCollection networkAdapterAddresses = ipProperties.UnicastAddresses;

                    foreach (UnicastIPAddressInformation networkAdapterAddressInformation in networkAdapterAddresses)
                    {
                        serverIpAddress = serverAddressCollection.NewIpAddress();
                        serverIpAddress.IpAddressObject = DynDnsIpAddressObject.ServiceDNS;
                        serverIpAddress.IpAddress = networkAdapterAddressInformation.Address.ToString();

                        if (
                            serverIpAddress.IpAddressType != DynDnsIpAddressType.NotValid /* valid IP */ &&
                           (
                               networkAdapterAddressInformation.SuffixOrigin == SuffixOrigin.LinkLayerAddress ||
                               networkAdapterAddressInformation.SuffixOrigin == SuffixOrigin.Manual ||
                               networkAdapterAddressInformation.SuffixOrigin == SuffixOrigin.OriginDhcp
                           ) &&
                            networkAdapterAddressInformation.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred
                        )
                        {
                            serverAddressCollection.Add(serverIpAddress);
                        }
                    }
                }
            }

            serverAddressCollection.WriteIpAddressCollection();

            // update IP in Domain
            this.UpdateLocalDomainAddress(serverAddressCollection);

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
