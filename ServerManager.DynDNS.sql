select c.DynDnsIpAddressType_Name, a.DynDnsIpAddress_Name
from DynDnsIpAddress a
inner join DynDnsService b on
    a.DynDnsIpAddressReferenceType_ID = 1 and b.DynDnsService_Name = 'shinkansen' and
	a.DynDnsIpAddressReference_ID = b.DynDnsService_ID
inner join DynDnsIpAddressType c on
    a+.DynDnsIpAddressType_ID = c.DynDnsIpAddressType_ID