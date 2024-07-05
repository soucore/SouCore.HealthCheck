using SampleWorker.CustomizedHealthCheck;
using Soucore.HealthCheck;

namespace SampleWorker;
public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
                services.AddTransient<InjectableInstance>();
                services.AddViaHealthCheck((_, probes) =>
                {
                    probes.AddDependency<CustomHealthCheckAuto<InjectableInstance>>("kafka-custom-hlg");
                    probes.AddDependency<CustomHealthCheckAuto<InjectableInstance>>("kafka-pedidos-hlg");
                    probes.AddDependency<CustomHealthCheck>("mongo-pedidos");
                    probes.AddDependency<CustomHealthCheck>("mongo-atlas-off");

                    probes.AddHealthCheck(8084, "/liveness", new string[] { "kafka-custom-hlg", "kafka-pedidos-hlg" });
                    probes.AddHealthCheck(8085, "/readiness", true);
                    probes.AddHealthCheckPrometheus(8082, "/dependencies");
                });
            });
    }
}