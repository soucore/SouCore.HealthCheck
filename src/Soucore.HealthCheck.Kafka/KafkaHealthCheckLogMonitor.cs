using System;

namespace Soucore.HealthCheck.Kafka
{
    public static class KafkaHealthCheckLogMonitor
    {
        private static event Action<string> Listener;
        internal static bool StatusCurrent { get; private set; }
        private static bool IsUsed { get; set; }

        static KafkaHealthCheckLogMonitor()
        {
            Listener += VerifyFacilityLog;
        }

        public static bool Status(bool status = false)
        {
            if (!IsUsed) return status;
            return IsUsed && StatusCurrent;
        }

        public static void InvokeFacilityLog(string facility)
        {
            IsUsed = true;
            Listener?.Invoke(facility);
        }

        private static void VerifyFacilityLog(string facility)
        {
            var status = FacilityExtension.IsConnected(facility);
            if (status == -1) return;
            StatusCurrent = status == 1;
        }
    }
}