namespace blog.dachs.ServerManager.DynDNS
{
    using System.Data;
    using System.Net;

    public class DynDnsServiceLocal : DynDnsService
    {
        public DynDnsServiceLocal(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
        }

        public override void GetIpAddress()
        {
            // get public IP from parent
            DynDnsIpAddressCollection serverPublicIpAddressCollection = this.Parent.Parent.NewIpAddressCollection();
            DynDnsIpAddressCollection servicePublicIpAddressCollection = this.NewIpAddressCollection();

            serverPublicIpAddressCollection.ReadIpAddressCollection(DynDnsIpAddressObject.ServiceDNS, DynDnsIpAddressType.Public);
            servicePublicIpAddressCollection.Add(serverPublicIpAddressCollection);
            servicePublicIpAddressCollection.WriteIpAddressCollection();

            // get private IP
            base.GetPrivateIpAddress();
        }

        public override void UpdatePublicDnsIpAddress()
        {
            DynDnsIpAddressCollection servicePublicIpAddressCollection;

            servicePublicIpAddressCollection = this.NewIpAddressCollection();
            servicePublicIpAddressCollection.ReadIpAddressCollection(DynDnsIpAddressObject.ServiceDNS, DynDnsIpAddressType.Public);

            foreach (DynDnsIpAddress ipAddress in servicePublicIpAddressCollection)
            {
                ipAddress.IpAddressObject = DynDnsIpAddressObject.UpdatedIP;
                ipAddress.PrepareIpAddressToDisc();

                ipAddress.IpAddressObject = DynDnsIpAddressObject.ValidatedIP;
                ipAddress.PrepareIpAddressToDisc();
            }

            base.GetPublicIpAddress(DynDnsIpAddressObject.ValidatedIP);
        }

        public override void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
            int ipAddressID = 0;
            string dynDnsIpAddressNetworkID = string.Empty;
            string sqlCommand = string.Empty;
            DataTable dataTable = null;
            DataRow dataRow = null;
            string url;
            DynDnsIpAddress ipAddress;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "checking for updateable Public IPs (" + ipAddressVersion.ToString() + ") for " + this.Name));

            sqlCommand = Database.GetCommand(Command.DynDnsServiceLocal_UpdatePublicDnsIpAddress_ReadIpAddressID);
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressReferenceID>", DynDnsServiceID.ToString());
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressObjectID_1>", Convert.ToString((byte)DynDnsIpAddressObject.ServiceDNS));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressObjectID_2>", Convert.ToString((byte)DynDnsIpAddressObject.ValidatedIP));
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressVersionID>", Convert.ToByte(ipAddressVersion).ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, sqlCommand));

            dataTable = Database.GetDataTable(sqlCommand);
            dataRow = null;


            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                ipAddressID = Convert.ToInt32(dataRow[0].ToString());
                ipAddress = new DynDnsIpAddress(this.Configuration, ipAddressID);
                ipAddress.ReferenceType = DynDnsIpAddressReferenceType.DynDnsService;
                ipAddress.ReferenceId = this.DynDnsServiceID;

                // update IP if row exists
                url = string.Empty;

                url = updateUri;
                url = url.Replace("<user>", networkCredential.UserName.Decrypt());
                url = url.Replace("<password>", networkCredential.Password.Decrypt());
                url = url.Replace("<servicename>", Name);


                if (ipAddress != null && ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv4)
                {
                    url = url.Replace("<ip4addr>", ipAddress.IpAddress.ToString());
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPv4 IP for " + this.Name + " to " + ipAddress.IpAddress.ToString()));
                    this.WebRequest.Request(url, networkCredential, ipAddressVersion);

                    ipAddress.IpAddressObject = DynDnsIpAddressObject.UpdatedIP;
                    ipAddress.ChangeDate = DateTime.Now;
                    ipAddress.WriteIpAddress();
                }

                if (ipAddress != null && ipAddress.IpAddressVersion == DynDnsIpAddressVersion.IPv6)
                {
                    url = url.Replace("<ip6addr>", ipAddress.IpAddress.ToString());
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPv6 IP for " + this.Name + " to " + ipAddress.IpAddress.ToString()));
                    this.WebRequest.Request(url, networkCredential, ipAddressVersion);

                    ipAddress.IpAddressObject = DynDnsIpAddressObject.UpdatedIP;
                    ipAddress.ChangeDate = DateTime.Now;
                    ipAddress.WriteIpAddress();
                }
            }
        }
    }
}
