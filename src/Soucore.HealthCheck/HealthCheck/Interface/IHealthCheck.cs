using System.Threading.Tasks;

namespace Soucore.HealthCheck.HealthCheck.Interface
{
    public interface IHealthCheck
    {
        bool IsHealthy { get; }
        Task Healthy();
        Task Unhealthy();
    }
}