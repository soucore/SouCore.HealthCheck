using HealthCheck.Sample;
using Soucore.HealthCheck;
using Soucore.HealthCheck.MongoDb;
using Soucore.HealthCheck.Test.Dependencies;

namespace HealthCheck.MongoDb.Sample;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Result.Build().Run();
    }

    public static async Task<IHostBuilder> CreateHostBuilder(string[] args)
    {
        var mongoContainer1 = new MongoDbContainerDependency(port:6556);
        await mongoContainer1.InitializeAsync();

        var mongoContainer2 = new MongoDbContainerDependency(port: 6557);
        await mongoContainer2.InitializeAsync();

        //var mongoContainer3 = new MongoDbContainerDependency(PORT: 6558);
        //await mongoContainer3.InitializeAsync();

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton(x =>
                {
                    return mongoContainer1.Client;
                });
                services.AddSingleton(x =>
                {
                    return new HealthCheck.Sample.MongoConnection2(mongoContainer2.Testcontainers.ConnectionString);
                });
                //services.AddSingleton(x =>
                //{
                //    return new MongoConnection3(mongoContainer3.Testcontainers.ConnectionString);
                //});

                services.AddViaHealthCheck((services, probes) =>
                {
                    probes.AddMongoHealthCheck("key1", (config, provider) => 
                    {
                        config.MongoClientSettings = mongoContainer1.Client.Settings;
                    });
                    probes.AddMongoHealthCheck("key2", mongoContainer2.Client.Settings);
                    //probes.AddMongoHealCheck("key3", mongoContainer3.Client.Settings);

                    probes.AddHealthCheck(8081, "/readiness");
                    probes.AddHealthCheck(8080, "/liveness", true);
                    probes.AddHealthCheckPrometheus(8082, "/dependencies");
                });
            });
    }
}