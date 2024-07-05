using Confluent.Kafka;
using System;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Kafka
{
    public static class Extension
    {
        public static Probe AddKafkaHealthCheck(this Probe probe, string alias, Action<KafkaSettings> setupAction)
        {
            probe.AddDependency<KafkaHealthCheck, KafkaSettings>(setupAction, alias);
            return probe;
        }

        public static Probe AddKafkaHealthCheck(this Probe probe, string alias, Action<KafkaSettings, IServiceProvider> setupAction)
        {
            probe.AddDependency<KafkaHealthCheck, KafkaSettings>(setupAction, alias);
            return probe;
        }

        public static Probe AddKafkaHealthCheckConsumer<TKey, TValue>(this Probe probe, string alias = null)
        {
            probe.AddDependency<KafkaHealthCheck<TKey, TValue>>(alias);
            return probe;
        }

        public static Probe AddKafkaHealthCheckLogMonitor(this Probe probe, string alias = null)
        {
            probe.AddDependency<KafkaHealthCheck<Ignore, string>>(alias);
            return probe;
        }

        public static Probe AddKafkaHealthCheckLogMonitor(this Probe probe, string alias, Action<DefaultCustomSettings> setupAction)
        {
            probe.AddDependency<KafkaHealthCheck<Ignore, string>, DefaultCustomSettings>(setupAction, alias);
            return probe;
        }
    }
}
