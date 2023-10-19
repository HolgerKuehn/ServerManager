
using System.Data;
using System.Net;

namespace blog.dachs.ServerManager
{
    public class DynDnsServiceLocal : DynDnsService
    {
        public DynDnsServiceLocal(Configuration configuration, int dynDnsServiceID) : base(configuration, dynDnsServiceID)
        {
            
        }

        public override void GetIpAddress()
        {
            // get public IP 
            base.GetPublicIpAddress();
        }

        public override void UpdatePublicDnsIpAddress()
        {
            base.UpdatePublicDnsIpAddress();
        }

        public override void UpdatePublicDnsIpAddress(string updateUri, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion)
        {
            string dynDnsIpAddressIpAddressID = string.Empty;
            string dynDnsIpAddressNetworkID = string.Empty;
            string sqlCommand = string.Empty;
            DataTable dataTable = null;
            DataRow dataRow = null;

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "checking for updateable Public IPs (" + ipAddressVersion.ToString() + ") for " + this.Name));

            sqlCommand = this.HandlerDatabase.GetCommand(Command.DynDnsServiceLocal_UpdatePublicDnsIpAddress_ReadIpAddressID);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", this.DynDnsServiceID.ToString());
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressVersionID>", Convert.ToByte(ipAddressVersion).ToString());

            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, sqlCommand));

            dataTable = this.HandlerDatabase.GetDataTable(sqlCommand);
            dataRow = null;

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                dataRow = dataTable.Rows[row];
                dynDnsIpAddressIpAddressID = dataRow[0].ToString();
                dynDnsIpAddressNetworkID = dataRow[1].ToString();

                // update IP if row exists
                string url = string.Empty;
                DynDnsIpAddressCollection ipAddressCollection;

                url = updateUri;
                url = url.Replace("<user>", networkCredential.UserName);
                url = url.Replace("<password>", networkCredential.Password);
                url = url.Replace("<servicename>", this.Name);

                ipAddressCollection = this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv4);

                if (ipAddressCollection.Count > 0)
                {
                    url = url.Replace("<ip4addr>", ipAddressCollection.ElementAt(0).IpAddress.ToString());
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPv4 IP for " + this.Name + " to " + ipAddressCollection.ElementAt(0).IpAddress.ToString()));
                }
                else
                {
                    url = url.Replace("<ip4addr>", "127.0.0.1");
                }

                ipAddressCollection = this.IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv6);
                if (ipAddressCollection.Count > 0)
                {
                    url = url.Replace("<ip6addr>", ipAddressCollection.ElementAt(0).IpAddress.ToString());
                    this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPV6 IP for " + this.Name + " to " + ipAddressCollection.ElementAt(0).IpAddress.ToString()));
                }
                else
                {
                    url = url.Replace("<ip6addr>", "::1");
                }

                this.HandlerWebRequest.Request(url, networkCredential, ipAddressVersion);


                this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "set Public IPs for " + this.Name + " as updated"));

                // IP Address
                if (dynDnsIpAddressIpAddressID != null)
                    this.UpdatePublicDnsIpAddress(dynDnsIpAddressIpAddressID);

                // Network Address
                if (dynDnsIpAddressNetworkID != null)
                    this.UpdatePublicDnsIpAddress(dynDnsIpAddressNetworkID);
            }
        }
    }
}
