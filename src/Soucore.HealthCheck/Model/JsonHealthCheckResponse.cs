using System.Net;
using System.Text.Json;

namespace Soucore.HealthCheck.Model
{
    internal sealed class JsonHealthCheckResponse : HealthCheckResponse
    {
        private const string Type = "application/json";

        public JsonHealthCheckResponse(StatusResponse body) : base(body, Type) { }
        public JsonHealthCheckResponse(StatusResponse body, HttpStatusCode statusCode) : base(body, Type, statusCode) { }
        protected override string Serialize(StatusResponse body) => JsonSerializer.Serialize(body);
    }
}