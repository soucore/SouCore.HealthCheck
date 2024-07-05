using System;

namespace Soucore.HealthCheck.Kafka
{
    public static class FacilityExtension
    {
        public static short IsConnected(this string facility)
        {
            if (Enum.TryParse(facility, out ListenerStatus statusEnum))
            {
                if (!(ListenerStatus.ERROR & ListenerStatus.FAIL & ListenerStatus.MAXPOLL).HasFlag(statusEnum))
                    return 0;

                if (statusEnum == ListenerStatus.CONNECTED)
                    return 1;
            }

            return -1;
        }
    }
}
