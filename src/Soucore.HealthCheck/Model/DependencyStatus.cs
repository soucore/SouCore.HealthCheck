using System;

namespace Soucore.HealthCheck.Model
{
    public sealed class DependencyStatus
    {
        public string Alias { get; set; }
        public bool Status { get; set; }
        public DateTime? LastHealthyTime { get; set; }
        public DateTime LastCheck { get; internal set; }
        public bool Disabled { get; set; }
    }
}
