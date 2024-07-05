using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace SampleWorker.CustomizedHealthCheck;

internal class CustomHealthCheck : IHealthCheckCustom
{
    public bool Disabled { get; set; }

    public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
    {
        return await Task.FromResult(new HealthCheckResult(true));
    }
}
