using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Soucore.HealthCheck.HealthCheck;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;
using Soucore.HealthCheck.Services;

namespace Soucore.HealthCheck
{
    public static class Extension
    {
        public static IServiceCollection AddViaHealthCheck(this IServiceCollection services, Action<IServiceCollection, Probe> probesFunc, string hostname = "*")
        {
            AddViaHealthCheck(services, probesFunc, (settings) =>
            {
                settings.Hostname = hostname;
                return settings;
            });

            return services;
        }

        public static IServiceCollection AddViaHealthCheck(this IServiceCollection services, Action<IServiceCollection, Probe> probesFunc, Func<HealthCheckSettings, HealthCheckSettings> settingsFunc)
        {
            var settings = settingsFunc(new HealthCheckSettings());
            services.TryAddSingleton(settings);
            services.TryAddSingleton<IHealthCheck, WorkerHealthCheck>();
            probesFunc(services, new Probe(services, settings));
            services.AddHostedService<HealthCheckDependencyService>();

            return services;
        }

        public static IServiceCollection AddViaHealthCheckCustom<T>(this IServiceCollection services) where T : IHealthCheckCustom
        {
            return services.AddSingleton<IHealthCheckCustom>(x =>
            {
                return ActivatorUtilities.CreateInstance<T>(x);
            });
        }
    }
}