using System.Collections.Generic;

namespace Soucore.HealthCheck.Model
{
    public class StatusResponse
    {
        public bool Status { get; }
        public IEnumerable<DependencyStatus> DependenciesStatus { get; } = new List<DependencyStatus>();

        public StatusResponse(bool status)
        {
            Status = status;
        }

        public StatusResponse(bool status, IEnumerable<DependencyStatus> dependenciesStatus)
        {
            Status = status;
            DependenciesStatus = dependenciesStatus;
        }
    }
}