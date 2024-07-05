using System.Net;
using System.Text;

namespace Soucore.HealthCheck.Model
{
    internal abstract class HealthCheckResponse
    {
        protected HealthCheckResponse(StatusResponse body, string contentType)
        {
            Body = Serialize(body);
            ContentType = contentType;
            StatusCode = HttpStatusCode.OK;
        }

        protected HealthCheckResponse(StatusResponse body, string contentType, HttpStatusCode statusCode)
        {
            Body = Serialize(body);
            ContentType = contentType;
            StatusCode = statusCode;
        }

        public string Body { get; }
        public HttpStatusCode StatusCode { get; }
        public string ContentType { get; }
        public Encoding ContentEncoding { get; } = Encoding.UTF8;
        protected abstract string Serialize(StatusResponse body);
    }
}