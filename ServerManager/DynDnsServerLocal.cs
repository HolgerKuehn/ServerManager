using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace blog.dachs.ServerManager
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
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        DynDnsIpAddress networkIPAddress = new DynDnsIpAddress(this.Configuration, dnsAdress.ToString());
                        if (networkIPAddress.IpAddressType != DynDnsIpAddressType.NotValid)
                        {
                            UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = ipProperties.UnicastAddresses;
                            foreach (UnicastIPAddressInformation unicastIPAddressInformation in unicastIPAddressInformationCollection)
                            {
                                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && networkIPAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                                {
                                    networkIPAddress.IpAddressType = DynDnsIpAddressType.DnsServerPrivate;
                                    networkIPAddress.PrefixLength = (byte)unicastIPAddressInformation.PrefixLength;
                                }

                                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetworkV6 && networkIPAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                                {
                                    if (networkIPAddress.IpAddressType == DynDnsIpAddressType.LinkLocal)
                                    {
                                        networkIPAddress.IpAddressType = DynDnsIpAddressType.DnsServerLinkLocal;
                                    }
                                    else if (networkIPAddress.IpAddressType == DynDnsIpAddressType.UniqueLocal)
                                    {
                                        networkIPAddress.IpAddressType = DynDnsIpAddressType.DnsServerUniqueLocal;
                                    }

                                    networkIPAddress.PrefixLength = (byte)unicastIPAddressInformation.PrefixLength;
                                }
                            }

                            this.IpAddressCollection.Add(networkIPAddress);
                        }
                    }
                }
            }

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServerLocal_DynDnsServerLocal, "create new ThreadFirewallRuleBaseProperties"));
            this.Configuration.ThreadCollection.ThreadFirewallRuleBaseProperties(configuration, this);
        }

        public override void UpdatePublicDnsIpAddress()
        {
            base.UpdatePublicDnsIpAddress();

            this.UpdatePublicDnsIpAddress(Command.DynDnsService_UpdatePublicDnsIpAddress_ReadIpAddressIDPublicIp, "set public IPs from " + this.Name + " as updated");
        }
    }
}
