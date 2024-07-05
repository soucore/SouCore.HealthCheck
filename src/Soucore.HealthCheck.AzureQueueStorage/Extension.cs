using System;

namespace Soucore.HealthCheck.AzureQueueStorage
{
    public static class Extension
    {
        public static Probe AddAzureQueueStorageHealthCheck(this Probe probe, string alias, Action<AzureQueueStorageHealthCheckSettings> setupAction)
        {
            probe.AddDependency<AzureQueueStorageHealthCheck, AzureQueueStorageHealthCheckSettings>(setupAction, alias);
            return probe;
        }

        public static Probe AddAzureQueueStorageHealthCheck(this Probe probe, string alias, Action<AzureQueueStorageHealthCheckSettings, IServiceProvider> setupAction)
        {
            probe.AddDependency<AzureQueueStorageHealthCheck, AzureQueueStorageHealthCheckSettings>(setupAction, alias);
            return probe;
        }
    }
}