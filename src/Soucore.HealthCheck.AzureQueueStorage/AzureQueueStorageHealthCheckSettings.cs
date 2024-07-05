using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.AzureQueueStorage
{
    //TODO: Config.Disable
    //Os outros arquivos de configurações dos HealthCheck, precisam ser modificados tbm
    public class AzureQueueStorageHealthCheckSettings : DefaultCustomSettings
    {
        public string ConnectionString { get; set; }
        public string Queue { get; set; }
    }
}