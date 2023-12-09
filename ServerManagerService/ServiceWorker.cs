using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace blog.dachs.ServerManager
{
    public class ServiceWorker : BackgroundService
    {
        private readonly ILogger<ServiceWorker> logger;

        public ServiceWorker(ILogger<ServiceWorker> logger, IHostEnvironment environment)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ServerManagerService started.");
            Configuration configuration = new Configuration();


            while (!stoppingToken.IsCancellationRequested)
            {
                
                await Task.Delay(1, stoppingToken);
            }
        }
    }

}
