using Soucore.HealthCheck.HealthCheck.Interface;

namespace HealthCheck.Sample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHealthCheck _healthcheck;
        public DateTime Time { get; set; } = DateTime.Now;

        public Worker(ILogger<Worker> logger, IHealthCheck healthcheck)
        {
            _logger = logger;
            _healthcheck = healthcheck;
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
                    await Task.Delay(5000, stoppingToken);

                    await _healthcheck.Healthy();
                }
                catch (Exception)
                {
                    await _healthcheck.Unhealthy();
                }
            }
        }
    }
}