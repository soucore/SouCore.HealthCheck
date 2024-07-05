using Confluent.Kafka;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Kafka;

namespace HealthCheck.Kafka.Sample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHealthCheck _healthCheck;
        private readonly Settings _settings;

        public Worker(ILogger<Worker> logger, IHealthCheck healthCheck, Settings settings)
        {
            _logger = logger;
            _healthCheck = healthCheck;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consumer Started!");
            await _healthCheck.Healthy();
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServer,
                GroupId = "healthcheck-kafka",
                Debug = "protocol"
            };
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config)
                    .SetLogHandler((e, l) => KafkaHealthCheckLogMonitor.InvokeFacilityLog(l.Facility))
                    .Build();
                consumer.Subscribe("healthcheck");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    _logger.LogInformation("message", consumeResult.Message.Value);

                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                await _healthCheck.Unhealthy();
                _logger.LogError("error", ex.Message);
            }
        }
    }
}