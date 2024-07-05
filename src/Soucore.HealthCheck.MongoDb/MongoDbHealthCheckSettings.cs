using MongoDB.Driver;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.MongoDb
{
    public class MongoDbHealthCheckSettings : DefaultCustomSettings
    {
        public string ConnectionString { get; set; }
        public MongoClientSettings MongoClientSettings { get; set; }
    }
}
