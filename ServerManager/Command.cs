namespace blog.dachs.ServerManager
{
    public enum Command
    {
        Configuration_Configuration = 1,
        ThreadDynDns_ThreadDynDns_DynDnsServiceType = 2,
        ThreadDynDns_ThreadDynDns_DynDnsServer = 3,
        DynDnsDomain_DynDnsDomain = 4,
        DynDnsServer_DynDnsServer = 5,
        GuiWindowLog_TmrGuiWindowLog_Tick = 6,
        DynDnsService_GetDnsIpAddress = 7,
        DynDnsService_DynDnsService = 8,
        DynDnsService_PerpareIpAddressCollectionToDisc_IpAddress = 9,
        DynDnsService_PerpareIpAddressCollectionToDisc_NetworkAddress = 10
    }
}
