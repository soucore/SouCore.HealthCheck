using System;

namespace Soucore.HealthCheck.Model
{
    public sealed class HealthCheckResult
    {
        public bool Status { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }

        public HealthCheckResult(bool status, Exception exception) : this(status, string.Empty, exception) {}
        public HealthCheckResult(bool status) : this(status, string.Empty) {}
        public HealthCheckResult(bool status, string messageIsFalse) : this(status, messageIsFalse, null) {}
        public HealthCheckResult(bool status, string messageIsFalse, Exception exception)
        {
            Status = status;
            Message = MessageHandle(status, messageIsFalse);
            Exception = exception;
        }


        private string MessageHandle(bool status, string message)
        {
            if (status) return "Healthy!";
            return string.Concat("Unheathy! ", message);
        }
    }
}
