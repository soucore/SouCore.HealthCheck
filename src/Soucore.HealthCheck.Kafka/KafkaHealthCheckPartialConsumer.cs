using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Kafka
{
    public partial class KafkaHealthCheck<TKey, TValue> : IHealthCheckCustom
    {
        private readonly ILogger<KafkaHealthCheck<TKey, TValue>> _logger;
        private readonly IServiceProvider _provider;
        private readonly DefaultCustomSettings _settings;

        public bool Disabled { get; set; }

        public KafkaHealthCheck(ILogger<KafkaHealthCheck<TKey, TValue>> logger, IServiceProvider provider, DefaultCustomSettings settings)
        {
            _logger = logger;
            _provider = provider;
            _settings = settings;
            Disabled = _settings.Disable;
        }

#pragma warning disable CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
#pragma warning restore CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
        {
            try
            {
                bool result;
                var consumer = _provider.GetService<IConsumer<TKey, TValue>>();
                if (consumer is null)
                {
                    result = KafkaHealthCheckLogMonitor.Status();
                    return new HealthCheckResult(result, "Facility not CONECTED!");
                }

                result = consumer.Assignment.Count > 0 && KafkaHealthCheckLogMonitor.Status(true);
                return new HealthCheckResult(result);
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(false, ex);
            }
        }
    }
}
