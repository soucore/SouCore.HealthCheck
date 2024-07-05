using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;
using Soucore.HealthCheck.Services;

namespace Soucore.HealthCheck.HealthCheck
{
    internal sealed class Default : IHealthCheckService
    {
        private readonly IHealthCheck _healthCheck;

        public Default(IHealthCheck healthCheck)
        {
            _healthCheck = healthCheck;
        }

#pragma warning disable CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
        public async Task<HealthCheckResponse> ExecuteAsync(string[] alias, CancellationToken cancellationToken)
#pragma warning restore CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
        {
            var dependencies = HealthCheckDependencyService.GetDependencyStatus(alias);
            if (!dependencies.Any(x => !x.Status) && _healthCheck.IsHealthy)
                return new JsonHealthCheckResponse(new StatusResponse(status: true, dependencies), HttpStatusCode.OK);

            return new JsonHealthCheckResponse(new StatusResponse(status: false, dependencies), HttpStatusCode.InternalServerError);
        }
    }
}