using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.HealthCheck.Interface
{
    public interface IHealthCheckCustom
    {
        Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken);

        bool Disabled { get; set; }
    }
}
