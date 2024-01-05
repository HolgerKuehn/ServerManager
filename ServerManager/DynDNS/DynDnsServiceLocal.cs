
using System.Data;
using System.Net;
using blog.dachs.ServerManager;

namespace blog.dachs.ServerManager.DynDNS
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

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "checking for updateable Public IPs (" + ipAddressVersion.ToString() + ") for " + Name));

            sqlCommand = Database.GetCommand(Command.DynDnsServiceLocal_UpdatePublicDnsIpAddress_ReadIpAddressID);
            sqlCommand = sqlCommand.Replace("<DynDnsServiceID>", DynDnsServiceID.ToString());
            sqlCommand = sqlCommand.Replace("<DynDnsIpAddressVersionID>", Convert.ToByte(ipAddressVersion).ToString());

            Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, sqlCommand));

            dataTable = Database.GetDataTable(sqlCommand);
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
                url = url.Replace("<servicename>", Name);

                ipAddressCollection = IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv4);

                if (ipAddressCollection.Count > 0)
                {
                    url = url.Replace("<ip4addr>", ipAddressCollection.ElementAt(0).IpAddress.ToString());
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPv4 IP for " + Name + " to " + ipAddressCollection.ElementAt(0).IpAddress.ToString()));
                }
                else
                {
                    url = url.Replace("<ip4addr>", "127.0.0.1");
                }

                ipAddressCollection = IpAddressCollection.GetIpAddressCollection(DynDnsIpAddressVersion.IPv6);
                if (ipAddressCollection.Count > 0)
                {
                    url = url.Replace("<ip6addr>", ipAddressCollection.ElementAt(0).IpAddress.ToString());
                    Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "updated Public IPV6 IP for " + Name + " to " + ipAddressCollection.ElementAt(0).IpAddress.ToString()));
                }
                else
                {
                    url = url.Replace("<ip6addr>", "::1");
                }

                WebRequest.Request(url, networkCredential, ipAddressVersion);


                Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.DynDnsServiceLocal_UpdatePublicDnsIpAddress, "set Public IPs for " + Name + " as updated"));

                // IP Address
                if (dynDnsIpAddressIpAddressID != null)
                    UpdatePublicDnsIpAddress(dynDnsIpAddressIpAddressID);

                // Network Address
                if (dynDnsIpAddressNetworkID != null)
                    UpdatePublicDnsIpAddress(dynDnsIpAddressNetworkID);
            }
        }
    }
}
