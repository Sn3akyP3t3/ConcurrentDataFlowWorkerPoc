namespace ConcurrentDataflowWorkerPoc
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceRunner _serviceRunner;

        public Worker(ILogger<Worker> logger, IServiceRunner serviceRunner)
        {
            _logger = logger;
            _serviceRunner = serviceRunner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _serviceRunner.DoProcess();

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
