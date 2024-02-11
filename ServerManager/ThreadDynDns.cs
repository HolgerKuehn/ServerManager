﻿/// <summary>
/// Namespace for ServiceManager
/// Copyright Holger Kühn, 2023
/// </summary>
namespace blog.dachs.ServerManager
{
    using blog.dachs.ServerManager.DynDNS;
    using System.Data;

    /// <summary>
    /// BaseClass for DynDns client
    /// </summary>
    public class ThreadDynDns : ThreadWorker
    {
        /// <summary>
        /// server running the DynDns client
        /// </summary>
        private DynDnsServer dynDnsServer;

        public ThreadDynDns(Configuration configuration) : base(configuration)
        {
            // reading server type (local or remote)
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading required server type"));
            
            string sqlCommand = this.Database.GetCommand(Command.ThreadDynDns_ThreadDynDns_DynDnsServiceType);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            DataRow dataRow = this.Database.GetDataRow(sqlCommand, 0);
            DynDnsServiceType dynDnsServiceType = DynDnsServiceType.Server;

            if (dataRow != null )
            {
                dynDnsServiceType = (DynDnsServiceType)Convert.ToInt32(dataRow[0].ToString());
            }


            // create server accordingly
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ThreadDynDns_ThreadDynDns, "reading DynDnsServer"));

            sqlCommand = this.Database.GetCommand(Command.ThreadDynDns_ThreadDynDns_DynDnsServer);
            sqlCommand = sqlCommand.Replace("<ConfigurationID>", this.Configuration.ConfigurationID.ToString());
            this.Configuration.GetLog().WriteLog(new LogEntry(LogSeverity.SQL, LogOrigin.ThreadDynDns_ThreadDynDns, sqlCommand));

            dataRow = this.Database.GetDataRow(sqlCommand, 0);
            int dynDnsServiceID = 0;

            // get DynDnsServiceID for server 
            if (dataRow != null)
            {
                dynDnsServiceID = Convert.ToInt32(dataRow[0].ToString());
            }

            // create server
            switch (dynDnsServiceType)
            {
                case DynDnsServiceType.ServiceLocal:
                    dynDnsServer = new DynDnsServerLocal(configuration, dynDnsServiceID);
                    break;

                case DynDnsServiceType.ServiceRemote:
                    dynDnsServer = new DynDnsServerRemote(configuration, dynDnsServiceID);
                    break;
            }
        }

        public DynDnsServer DynDnsServer
        {
            get { return this.dynDnsServer; }
            set { this.dynDnsServer = value; }
        }

        public override void Work()
        {
            this.DynDnsServer.SetDnsServer();
            
            while (true)
            {
                this.DynDnsServer.GetIpAddress();
            //  this.DynDnsServer.WriteLogForChangedIpAddress();
                this.DynDnsServer.UpdatePublicDnsIpAddress();
            
                Thread.Sleep(120000);
            }
        }
    }
}
