using Mongo2Go;
using MongoDB.Driver;
using Soucore.HealthCheck;
using Soucore.HealthCheck.MongoDb;

namespace HealthCheck.Sample;

public static class Program
{
    private static MongoDbRunner _runner;
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        _runner = MongoDbRunner.StartForDebugging();
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton(x =>
                {
                    _runner.Import("TestDatabase", "TestCollection", @"Test.json", true);
                    MongoClient client = new(_runner.ConnectionString);
                    return client;
                });
                services.AddSingleton(x =>
                {
                    MongoConnection2 client = new(_runner.ConnectionString);
                    return client;
                });

                _runner.Import("TestDatabase", "TestCollection3", @"Test.json", true);
                services.AddViaHealthCheck((services, probes) =>
                {
                    probes.AddMongoHealthCheck("key1", _runner.ConnectionString);
                    probes.AddMongoHealthCheck("key2", _runner.ConnectionString);

                    probes.AddMongoHealthCheck("key3", (config, _) =>
                    {
                        config.ConnectionString = _runner.ConnectionString;
                        config.Disable = true;
                    });

                    probes.AddHealthCheck(8001, "/readiness", new String[] { "key1", "key2" });
                    probes.AddHealthCheck(8000, "/liveness");
                    probes.AddHealthCheckPrometheus(8002, "/dependencies");
                    probes.AddHealthCheck(8006, "/readiness", new String[] { "key3" });
                });

            });
    }
}