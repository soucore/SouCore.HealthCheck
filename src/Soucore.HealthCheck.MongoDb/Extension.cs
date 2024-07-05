using MongoDB.Driver;
using System;

namespace Soucore.HealthCheck.MongoDb
{
    public static class Extension
    {

        public static Probe AddMongoHealthCheck(this Probe probe, string alias, string connectionString)
        {
            probe.AddDependency<MongoDbHealthCheck, MongoDbHealthCheckSettings>(config =>
            {
                config.ConnectionString = connectionString;
            }, alias);
            return probe;
        }

        public static Probe AddMongoHealthCheck(this Probe probe, string alias, MongoClientSettings mongoClientSettings)
        {
            probe.AddDependency<MongoDbHealthCheck, MongoDbHealthCheckSettings>(settings =>
            {
                settings.MongoClientSettings = mongoClientSettings;
            }, alias);
            return probe;
        }

        public static Probe AddMongoHealthCheck(this Probe probe, string alias, Action<MongoDbHealthCheckSettings, IServiceProvider> setupAction)
        {
            probe.AddDependency<MongoDbHealthCheck, MongoDbHealthCheckSettings>(setupAction, alias);
            return probe;
        }
    }
}