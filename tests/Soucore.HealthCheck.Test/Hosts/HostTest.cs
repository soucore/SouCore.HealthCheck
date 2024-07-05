using Microsoft.Extensions.Hosting;
using HealthCheck.Sample;

namespace Soucore.HealthCheck.Test.Hosts;

public class HostTest
{
    public IHostBuilder Host { get; set; }

    public async Task Setup()
    {
        Host = Program.CreateHostBuilder(new string[] { "localhost" });
        _ = Task.Run(() => { Host.Build().RunAsync(); });

        await Task.CompletedTask;
    }
}
