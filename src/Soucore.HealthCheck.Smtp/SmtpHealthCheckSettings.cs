using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Smtp
{
    public class SmtpHealthCheckSettings : DefaultCustomSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}