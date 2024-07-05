using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Soucore.HealthCheck.HealthCheck;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;
using Soucore.HealthCheck.Services;

namespace Soucore.HealthCheck
{
    public sealed class Probe
    {

        private readonly IServiceCollection _services;
        private readonly HealthCheckSettings _settings;

        private IList<string> Alias { get; set; } = new List<string>();

        
        public Probe(IServiceCollection services, HealthCheckSettings settings)
        {
            _services = services;
            _settings = settings;
        }

        public void AddDependency<T>() where T : IHealthCheckCustom
        {
            var alias = Guid.NewGuid().ToString();
            AddDependency<T>(alias);
        }

        public void AddDependency<T>(string alias) where T : IHealthCheckCustom
            => AddDependency<T, DefaultCustomSettings>(_ => new DefaultCustomSettings(), alias);

        /// <summary>
        /// Implement a configuration with IServiceProvider with opcional alias.
        /// </summary>
        public void AddDependency<T, TConfig>(Action<TConfig, IServiceProvider> actionConfig, string alias = null) where T : IHealthCheckCustom
        {
            var newAlias = alias ?? Guid.NewGuid().ToString();
            Alias.Add(alias);
            _services.AddSingleton<IWrapper>(provider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                actionConfig(config, provider);
                var healthCheck = ActivatorUtilities.CreateInstance<T>(provider, config);
                var wrapper = ActivatorUtilities.CreateInstance<WrapperHealthCheck<T>>(provider, healthCheck);
                wrapper.SetAlias(newAlias);

                return wrapper;
            });
        }

        /// <summary>
        /// Implement a configuration with IServiceProvider with opcional alias.
        /// </summary>
        public void AddDependency<T, TConfig>(Action<TConfig> actionConfig, string alias = null) where T : IHealthCheckCustom
        {
            var newAlias = alias ?? Guid.NewGuid().ToString();
            Alias.Add(alias);
            _services.AddSingleton<IWrapper>(provider =>
            {
                var config = Activator.CreateInstance<TConfig>();
                actionConfig(config);
                var healthCheck = ActivatorUtilities.CreateInstance<T>(provider, config);
                var wrapper = ActivatorUtilities.CreateInstance<WrapperHealthCheck<T>>(provider, healthCheck);
                wrapper.SetAlias(newAlias);

                return wrapper;
            });
        }

        public void AddHealthCheck(int port, string urlPath, string[] alias)
        {
            HealthCheckResponseConfiguration configuration(HealthCheckResponseConfiguration _)
            {
                return new HealthCheckResponseConfiguration
                {
                    Hostname = _settings.Hostname,
                    Port = port,
                    UrlPath = urlPath,
                    AliasName = alias
                };
            }

            AddWorker<HealthCheckService<Default>>(configuration);
        }

        public void AddHealthCheck(int port, string urlPath, bool allDependencies = false)
        {
            if (allDependencies)
                AddHealthCheck(port, urlPath, Alias.ToArray());
            else
                AddHealthCheck(port, urlPath, null);
        }

        public void AddHealthCheckPrometheus(int port, string urlPath)
        {
            HealthCheckResponseConfiguration Configuration(HealthCheckResponseConfiguration _)
            {
                return new HealthCheckResponseConfiguration
                {
                    Hostname = _settings.Hostname,
                    Port = port,
                    UrlPath = urlPath,
                    AliasName = Alias.ToArray()
                };
            }

            AddWorker<HealthCheckService<Dependency<PrometheusHealthCheckResponse>>>(Configuration);
        }

        private void AddWorker<T>(Func<HealthCheckResponseConfiguration, HealthCheckResponseConfiguration> configuration) where T : IHostedService
        {
            var type = typeof(T).GetGenericArguments()[0];
            _services.TryAddSingleton(type);
            _services.AddSingleton<IHostedService>(x =>
            {
                return ActivatorUtilities.CreateInstance<T>(x, configuration(new HealthCheckResponseConfiguration()));
            });
        }
    }
}
