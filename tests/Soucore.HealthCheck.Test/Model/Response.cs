namespace Soucore.HealthCheck.Test.Model;

public class Response
{
    public bool Status { get; set; }

    public List<DependencyStatus> DependenciesStatus { get; set; }

    public static Response FromPrometheusPayload(string payload) =>
        new Response
        {
            Status = ExtractStatuses(payload).All(x => x == 1)
        };

    private static IEnumerable<int> ExtractStatuses(string payload)
    {
        var statusArray = payload.TrimEnd().Split("\n")[2..];
        foreach (var status in statusArray)
            yield return int.Parse(status.Split(" ")[1]);
    }
}