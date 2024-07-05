using MongoDB.Driver;

namespace HealthCheck.MongoDb.Sample
{
    public class MongoConnection2 : MongoClient
    {
        public MongoConnection2(string connectionString)
            : base(connectionString) { }
    }
}
