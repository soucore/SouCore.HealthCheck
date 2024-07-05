using MongoDB.Driver;

namespace HealthCheck.MongoDb.Sample
{
    public class MongoConnection3 : MongoClient
    {
        public MongoConnection3(string connectionString)
            : base(connectionString) { }
    }
}
