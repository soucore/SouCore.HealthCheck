namespace Soucore.HealthCheck.Model
{
    public sealed class HealthCheckSettings
    {
        public string Hostname { get; set; } = "*";
        public int DependencyServiceSleep { get; set; } = 5000;
    }
}
