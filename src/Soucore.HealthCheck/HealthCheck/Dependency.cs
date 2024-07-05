using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;
using Soucore.HealthCheck.Services;

namespace Soucore.HealthCheck.HealthCheck
{
    internal class Dependency<T> : IHealthCheckService where T : HealthCheckResponse
    {
        public Task<HealthCheckResponse> ExecuteAsync(string[] alias, CancellationToken cancellationToken)
        {
            var statusCode = HealthCheckDependencyService.IsHealth
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;

            var statusResult = new StatusResponse(
                HealthCheckDependencyService.IsHealth,
                HealthCheckDependencyService.DependenciesStatus);

            var healthCheckResult = Activator.CreateInstance(typeof(T), statusResult, statusCode);
            return Task.FromResult((HealthCheckResponse)healthCheckResult);
        }
    }
}