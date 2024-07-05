using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.AzureQueueStorage
{


    internal class AzureQueueStorageHealthCheck : IHealthCheckCustom
    {
        private readonly ILogger<AzureQueueStorageHealthCheck> _logger;
        private readonly AzureQueueStorageHealthCheckSettings _settings;
        public bool Disabled { get; set; }

        public AzureQueueStorageHealthCheck(ILogger<AzureQueueStorageHealthCheck> logger, AzureQueueStorageHealthCheckSettings settings)
        {
            _logger = logger;
            _settings = settings;
            Disabled = _settings.Disable;
        }

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                QueueClient client = new QueueClient(_settings.ConnectionString, _settings.Queue);
                var result = await client.ExistsAsync(stoppingToken);
                string message = !result ? string.Empty : Message();

                return new HealthCheckResult(result, message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "HealthCheck - AzureQueueStorage unhealthy");
                return new HealthCheckResult(false, Message(), ex);
            }
        }


        private string Message()       
        {
            return string.Concat("Unable to check! ConnectionString: (", _settings.ConnectionString, ") | Queue: ", _settings.Queue);
        }
    }
}