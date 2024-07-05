using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck
{
    internal sealed class WrapperHealthCheck<T> : IHealthCheckCustom, IWrapper where T : IHealthCheckCustom
    {
        public string Alias => _alias;

        public bool Disabled { get; set; }

        private readonly T _healCheckCustom;
        public WrapperHealthCheck(T healCheckCustom)
        {
            _healCheckCustom = healCheckCustom;
            Disabled = _healCheckCustom.Disabled;
        }

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
        {
            return await _healCheckCustom.ExecuteAsync(stoppingToken);
        }

        private string _alias;


        public void SetAlias(string alias)
        {
            _alias = alias;
        }
    }
}
