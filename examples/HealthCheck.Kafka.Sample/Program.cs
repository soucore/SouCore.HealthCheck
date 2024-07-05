using Soucore.HealthCheck;
using Soucore.HealthCheck.Kafka;
using Soucore.HealthCheck.Test.Dependencies;

namespace HealthCheck.Kafka.Sample;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilderAsync(args).Result.Build().RunAsync();
    }

    public static async Task<IHostBuilder> CreateHostBuilderAsync(string[] args)
    {
        var kafka = new KafkaContainerDependency(zookeeperPort: 2182);
        await kafka.InitializeAsync();
        await kafka.CreateTopicAsync();

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton(x => new Settings()
                {
                    BootstrapServer = $"localhost:{kafka.Testcontainers.Port}"
                });

                //services.AddSingleton<IConsumer<Ignore, string>>(x => kafka.Consumer);

                services.AddViaHealthCheck((_, probe) =>
                {
                    //probe.AddKafkaHealthCheck("kafka", config =>
                    //{
                    //    config.BootstrapServer = $"localhost:{kafka.Testcontainers.Port}";
                    //    config.Topic = kafka.Topic;
                    //    config.Disable = true;
                    //});
                    //probe.AddKafkaHealthCheckConsumer<Ignore, string>();
                    probe.AddKafkaHealthCheckLogMonitor("kafka", config =>
                    {
                        config.Disable = true;
                    });
                    probe.AddHealthCheckPrometheus(8080, "/dependencies");
                    probe.AddHealthCheck(8084, "/liveness", true);
                });
                services.AddHostedService<Worker>();
            });
    }
}
