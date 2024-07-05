using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Kafka
{
    public class KafkaSettings : DefaultCustomSettings
    {
        public string BootstrapServer { get; set; }
        public string Topic { get; set; }
    }
}
