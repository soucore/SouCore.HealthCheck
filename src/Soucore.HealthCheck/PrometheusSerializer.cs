using System;
using System.Text;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Prometheus
{
    public static class PrometheusSerializer
    {
        public static string Serialize(StatusResponse instance)
        {
            var sb = new StringBuilder();
            const string metricName = "dependency_status";
            const string metricDescription = " Show if dependency is up(1) or down(0)\n";
            const string metricType = " gauge\n";

            sb.Append("# HELP ");
            sb.Append(metricName);
            sb.Append(metricDescription);

            sb.Append("# TYPE ");
            sb.Append(metricName);
            sb.Append(metricType);

            foreach (var dependency in instance.DependenciesStatus)
                sb.Append($"{metricName}{{alias=\"{dependency.Alias}\"}} {Convert.ToInt16(dependency.Status)}\n");

            return sb.ToString();
        }
    }
}