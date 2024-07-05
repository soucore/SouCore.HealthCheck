using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.Attributes;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Kafka
{
    [HealthCheckTimeout(5)]
    public class KafkaHealthCheck : IHealthCheckCustom
    {
        private IProducer<string, string> ClientBuilder { get; set; }
        private HealthCheckResult Status { get; set; }
        private event Action<string> EventSource;
        
        private readonly ILogger<KafkaHealthCheck> _logger;
        private readonly KafkaSettings _settings;

        public bool Disabled { get; set; }


        public KafkaHealthCheck(ILogger<KafkaHealthCheck> logger, KafkaSettings settings = null)
        {
            _logger = logger;
            _settings = settings;
            _settings.Topic ??= "via-healthcheck";
            EventSource += VerifyFacility;
            Disabled = _settings.Disable;
        }

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
            return Status;
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            if (_settings is null)
                await Task.CompletedTask;

            try
            {
                ClientBuilder ??= new ProducerBuilder<string, string>(new ProducerConfig()
                {
                    BootstrapServers = _settings.BootstrapServer,
                    Debug = "protocol"
                }).SetLogHandler((_, log) => EventSource?.Invoke(log.Facility))
                .Build();
                var result = await ClientBuilder.ProduceAsync(_settings.Topic, new Message<string, string>
                {
                    Key = "HealthCheck",
                    Value = "Health Check is Done!"
                }, stoppingToken);
                var resultHc = result?.Status != PersistenceStatus.NotPersisted;
                string message = !resultHc ? Message() : string.Empty;
                Status = new HealthCheckResult(resultHc, message);

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "HealthCheck - Kafka unhealthy");
                Status = new HealthCheckResult(false, ex);
            }
        }

        private void VerifyFacility(string facility)
        {
            var status = FacilityExtension.IsConnected(facility);
            if (status == -1) return;
            Status = new HealthCheckResult(status == 1, string.Concat("Facility status: ", status));
        }

        private string Message()
        {
            return string.Concat("Message show as Not Persisted in the kafka. BootstrapServers: (", _settings.BootstrapServer, ") | Topic: ", _settings.Topic);
        }
    }
}
