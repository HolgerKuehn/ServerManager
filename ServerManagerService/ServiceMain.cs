/// <summary>
/// Namespace for ServiceManager
/// Copyright Holger Kühn, 2023
/// </summary>
namespace blog.dachs.ServerManager
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.EventLog;

    /// <summary>
    /// BaseClass for Windows Service
    /// </summary>
    internal class ServiceMain
    {
        /// <summary>
        /// EntryPoint for Windows Service
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(options =>
                {
                    if (OperatingSystem.IsWindows())
                    {
                        options.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information);
                    }
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ServiceWorker>();

                    if (OperatingSystem.IsWindows())
                    {
                        services.Configure<EventLogSettings>(config =>
                        {
                            if (OperatingSystem.IsWindows())
                            {
                                config.LogName = "ServiceManagerService";
                                config.SourceName = "ServiceManager";
                            }
                        });
                    }
                })
                .UseWindowsService()
                .Build();

            host.Run();
        }
    }
}