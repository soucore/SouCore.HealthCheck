using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.Attributes;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Services
{
    internal sealed class HealthCheckDependencyService : BackgroundService
    {
        public static bool IsHealth { get; private set; }
        public static IList<DependencyStatus> DependenciesStatus { get; private set; }

        private readonly ILogger _logger;
        private readonly IEnumerable<IWrapper> _healthChecks;
        private readonly HealthCheckSettings _settings;

        public HealthCheckDependencyService(
            ILoggerFactory loggerFactory, 
            IEnumerable<IWrapper> wrapperHealthCheckCustom,
            HealthCheckSettings settings)
        {
            _logger = loggerFactory.CreateLogger("Soucore.HealthCheck");
            _logger.LogInformation("Starting dependency check service.");
            _healthChecks = wrapperHealthCheckCustom;
            _settings = settings;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                DependenciesStatus = BuildDependencies();

                _ = Task.Factory.StartNew(async () => {
                    await DoWork(stoppingToken);
                }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (ObjectDisposedException ex)
            {
                IsHealth = false;
                _logger.LogCritical(ex, "Error initiating thread dependencies.");
            }
            catch (Exception ex)
            {
                IsHealth = false;
                _logger.LogCritical(ex, "Error preparing dependencies for the service.");
            }

            await Task.CompletedTask;
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    DependenciesStatus ??= BuildDependencies();

                    foreach (var hc in _healthChecks)
                    {
                        var dependency = DependenciesStatus.Single(d => d.Alias == hc.Alias);
                        var health = (IHealthCheckCustom)hc;

                        if (dependency.Disabled = health.Disabled)
                        {
                            dependency.Status = true;
                            _logger.LogDebug("{alias} | {message}", dependency.Alias, "Disabled");
                            continue;
                        }

                        var timeout = GetTimeout(health);
                        var task = Task.Run(() => health.ExecuteAsync(stoppingToken));

                        task.Wait(timeout);
                        var result = await task;

                        dependency.Status = result.Status;
                        dependency.LastCheck = DateTime.Now;
                        dependency.LastHealthyTime = dependency.Status ? DateTime.Now : dependency.LastHealthyTime;
                        

                        _logger.LogDebug("{alias} | {message}", dependency.Alias, result.Message);
                        if (!dependency.Status)
                            _logger.LogError("{alias} | {status} | {message} | {ex}", dependency.Alias, result.Status, result.Message, result.Exception);
                    }

                    IsHealth = !DependenciesStatus.Any(x => !x.Status);
                    _logger.LogDebug("{isHealth} | All dependency check complete!", IsHealth);
                }
                catch (Exception ex)
                {
                    IsHealth = false;
                    _logger.LogError(ex, "Error processing health check dependencies");
                }

                Thread.Sleep(_settings.DependencyServiceSleep);
            }
        }

        public static IEnumerable<DependencyStatus> GetDependencyStatus(string[] alias)
        {
            var list = new List<DependencyStatus>(alias?.Length ?? 0);
            if (!alias?.Any() ?? true)
                return list;

            list = DependenciesStatus.Where(x => alias.Contains(x.Alias)).ToList();
            return list;
        }

        private TimeSpan GetTimeout(IHealthCheckCustom health)
        {
            Type instance = health.GetType().GenericTypeArguments[0];
            var time = instance?.GetCustomAttribute<HealthCheckTimeoutAttribute>()?.Timeout;

            return TimeSpan.FromSeconds(time ?? 2);
        }

        private IList<DependencyStatus> BuildDependencies()
        {
            var dependencies = new List<DependencyStatus>(_healthChecks.Count());
            foreach (var hc in _healthChecks)
            {
                dependencies.Add(new DependencyStatus
                {
                    Alias = hc.Alias
                });
            }

            return dependencies;
        }
    }
}
