using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;

namespace Soucore.HealthCheck.HealthCheck
{
    public sealed class WorkerHealthCheck : IHealthCheck
    {
        private readonly ILogger<WorkerHealthCheck> _logger;

        public WorkerHealthCheck(ILogger<WorkerHealthCheck> logger)
        {
            _logger = logger;
        }

        public bool IsHealthy { get; private set; }
        public async Task Healthy()
        {
            IsHealthy = true;
            _logger.LogDebug("Worker Healthy!");
            await Task.CompletedTask;
        }

        public async Task Unhealthy()
        {
            IsHealthy = false;
            _logger.LogDebug("Worker Unhealthy!");
            await Task.CompletedTask;
        }
    }
}