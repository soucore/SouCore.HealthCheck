using System;

namespace Soucore.HealthCheck.Kafka
{
    [Flags]
    public enum ListenerStatus
    {
        CONNECTED = 0b_0000_0000,
        ERROR = 0b_0000_0001,
        FAIL = 0b_0000_0010,
        MAXPOLL = 0b_0000_0100
    }
}
