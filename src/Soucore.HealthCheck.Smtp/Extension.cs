using System;

namespace Soucore.HealthCheck.Smtp
{
    public static class Extension
    {
        public static Probe AddSmtpHealthCheck(this Probe probe, string alias, Action<SmtpHealthCheckSettings> setupAction)
        {
            probe.AddDependency<SmtpHealthCheck, SmtpHealthCheckSettings>(setupAction, alias);
            return probe;
        }

        public static Probe AddSmtpHealthCheck(this Probe probe, string alias, Action<SmtpHealthCheckSettings, IServiceProvider> setupAction)
        {
            probe.AddDependency<SmtpHealthCheck, SmtpHealthCheckSettings>(setupAction, alias);
            return probe;
        }
    }
}