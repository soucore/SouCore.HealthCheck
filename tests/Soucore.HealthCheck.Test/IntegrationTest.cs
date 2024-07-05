using System.Text.Json;
using Soucore.HealthCheck.Test.Hosts;
using Soucore.HealthCheck.Test.Model;
using Xunit;
using Xunit.Priority;

namespace Soucore.HealthCheck.Test;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class IntegrationTest : IClassFixture<HostTest>
{
    private readonly HostTest _hostUp;
    private const int Port = 800;

    public IntegrationTest(HostTest hostUp)
    {
        _hostUp = hostUp;
        _ = _hostUp.Setup();
    }

    [Fact, Priority(1)]
    public async Task HealthCheck_ReturnsSuccessLiveness()
    {
        await Task.Delay(5000);
        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}0/liveness");
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(JsonSerializer.Deserialize<Response>(content).Status);
    }

    [Fact, Priority(2)]
    public async Task HealthCheck_ReturnsSuccessReadiness()
    {
        await Task.Delay(2000);
        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}1/readiness");
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(JsonSerializer.Deserialize<Response>(content).Status);
    }

    [Fact, Priority(3)]
    public async Task DependenciesHealthCheck_ReturnsSuccess()
    {
        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}2/dependencies");

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(Response.FromPrometheusPayload(content).Status);
    }

    [Fact, Priority(4)]
    public async Task HealthCheck_ReturnsFailReadiness()
    {
        await Task.Delay(16000);

        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}1/readiness");

        Assert.False(response.IsSuccessStatusCode);
        
        Assert.False(JsonSerializer.Deserialize<Response>(content).Status);
    }

    [Fact, Priority(5)]
    public async Task HealthCheck_ReturnsFailLiveness()
    {
        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}0/liveness");

        Assert.False(response.IsSuccessStatusCode);
        Assert.False(JsonSerializer.Deserialize<Response>(content).Status);
    }

    [Fact, Priority(6)]
    public async Task HealthCheck_ReturnsDisabledLiveness()
    {
        await Task.Delay(2000);

        var (content, response) = await Utils.QueryHealthCheckEndpoint($"http://localhost:{Port}6/readiness");

        Assert.True(JsonSerializer.Deserialize<Response>(content).DependenciesStatus[0].Status);
        Assert.True(JsonSerializer.Deserialize<Response>(content).DependenciesStatus[0].Disabled);
    }
}