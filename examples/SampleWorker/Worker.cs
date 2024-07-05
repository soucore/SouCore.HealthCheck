using Soucore.HealthCheck.HealthCheck.Interface;

namespace SampleWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHealthCheck _healthCheck;
        public DateTime Time { get; set; } = DateTime.Now;
        public Worker(ILogger<Worker> logger, IHealthCheck healthCheck)
        {
            _logger = logger;
            _healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (DateTime.Now > Time.AddSeconds(15))
                        throw new TimeoutException();

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    await _healthCheck.Healthy();
                }
                catch (Exception ex)
                {
                    await _healthCheck.Unhealthy();
                    _logger.LogError("{error}", ex.Message);
                    
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}