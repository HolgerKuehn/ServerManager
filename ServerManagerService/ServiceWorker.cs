/// <summary>
/// Namespace for ServiceManager
/// Copyright Holger Kühn, 2023
/// </summary>
namespace blog.dachs.ServerManager
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Worker for BaseClass for Windows Service
    /// </summary>
    public class ServiceWorker : BackgroundService
    {
        /// <summary>
        /// logger for EventLog
        /// </summary>
        private readonly ILogger<ServiceWorker> logger;

        /// <summary>
        /// Sets Logger for EventLog
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="environment"></param>
        public ServiceWorker(ILogger<ServiceWorker> logger, IHostEnvironment environment)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Working Task for Windows Service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ServerManagerService started.");
            Configuration configuration = new Configuration();

            try
            {
                // write Log in DataBase
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Informational, LogOrigin.ServiceWorker_Task, "starting ServerManager on \"" + Environment.MachineName + "\""));

                // create new Thread for DynDnsClient
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ServiceWorker_Task, "create new ThreadDynDns"));
                configuration.ThreadCollection.ThreadDynDns(configuration);

                // create new Thread for BackupClient
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Debug, LogOrigin.ServiceWorker_Task, "create new ThreadBackup"));
                configuration.ThreadCollection.ThreadBackup(configuration);
            }
            catch (Exception ex)
            {
                // write log, if an exception occurs
                configuration.GetLog().WriteLog(new LogEntry(LogSeverity.Critical, LogOrigin.ServiceWorker_Task, "caught exception: " + ex.Message.Replace("\"", "\"\"")));
            }

            // wait for cancellation requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(60000, stoppingToken);
            }

            // end all Threads
            if (stoppingToken.IsCancellationRequested)
            {
                configuration.ThreadCollection.TerminateThreads();
            }
        }
    }
}
