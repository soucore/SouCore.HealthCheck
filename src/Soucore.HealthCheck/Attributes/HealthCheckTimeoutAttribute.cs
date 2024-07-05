using System;

namespace Soucore.HealthCheck.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HealthCheckTimeoutAttribute : Attribute
    {
        public short Timeout { get; private set; }

        public HealthCheckTimeoutAttribute(short timeout)
        {
            Timeout = timeout;
        }
    }
}
