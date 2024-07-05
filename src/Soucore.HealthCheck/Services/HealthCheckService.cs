using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Services
{
    internal sealed class HealthCheckService<T> : BackgroundService where T : IHealthCheckService
    {
        private readonly ILogger<HealthCheckService<T>> _logger;
        private readonly T _healthCheck;
        private readonly HealthCheckResponseConfiguration _config;
        private readonly HttpListener _listener = new HttpListener();
        private CancellationTokenSource _tokenSource;
        public HealthCheckService(ILogger<HealthCheckService<T>> logger,
            T healthCheck,
            HealthCheckResponseConfiguration config)
        {
            _logger = logger;
            _healthCheck = healthCheck;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var listener = new HealthCheckHttpListener(_config, stoppingToken);
                listener.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HealthCheck HttpListener could not be started.");
            }
        }
    }
}
