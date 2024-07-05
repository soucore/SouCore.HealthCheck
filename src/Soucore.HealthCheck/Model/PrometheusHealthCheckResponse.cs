using System.Net;
using Soucore.HealthCheck.Prometheus;


namespace Soucore.HealthCheck.Model
{
    internal sealed class PrometheusHealthCheckResponse : HealthCheckResponse
    {
        private const string Type = "text/plain";

        public PrometheusHealthCheckResponse(StatusResponse body) : base(body, Type) { }
        public PrometheusHealthCheckResponse(StatusResponse body, HttpStatusCode statusCode) : base(body, Type, statusCode) { }

        protected override string Serialize(StatusResponse body) => PrometheusSerializer.Serialize(body);
        
    }
}