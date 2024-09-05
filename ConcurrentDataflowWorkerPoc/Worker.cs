namespace ConcurrentDataflowWorkerPoc
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDataWorkflowService _dataWorkflowService;

        public Worker(ILogger<Worker> logger, IDataWorkflowService dataWorkflowService)
        {
            _logger = logger;
            _dataWorkflowService = dataWorkflowService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _dataWorkflowService.DoProcess();

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
