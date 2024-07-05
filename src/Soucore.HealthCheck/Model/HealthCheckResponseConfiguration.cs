namespace Soucore.HealthCheck.Model
{
    public sealed class HealthCheckResponseConfiguration
    {
        public string Hostname { get; set; } = "*";
        public int Port { get; set; } = 80;
        public string UrlPath { get; set; } = "/health";
        public string[] AliasName { get; set; }
    }
}
