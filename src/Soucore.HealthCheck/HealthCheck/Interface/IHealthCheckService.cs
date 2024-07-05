using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.HealthCheck.Interface
{
    internal interface IHealthCheckService
    {
        Task<HealthCheckResponse> ExecuteAsync(string[] alias, CancellationToken cancellationToken);
    }
}
