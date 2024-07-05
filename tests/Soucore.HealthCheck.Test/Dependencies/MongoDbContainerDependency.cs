using AutoFixture;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using MongoDB.Driver;

using Xunit;

namespace Soucore.HealthCheck.Test.Dependencies
{
    public class MongoDbContainerDependency : IAsyncLifetime
    {
        public readonly TestcontainerDatabase Testcontainers;
        public IMongoClient Client;
        private readonly Fixture _fixture;

        public MongoDbContainerDependency(
            string database = "",
            string username = "",
            string password = "",
            int port = 27017)
        {
            _fixture = new Fixture();

            Testcontainers = new TestcontainersBuilder<MongoDbTestcontainer>()
                .WithExposedPort(port)
                .WithName($"mongo-{_fixture.Create<Guid>()}")
                .WithDatabase(new MongoDbTestcontainerConfiguration("mongo")
                {
                    Database = string.IsNullOrEmpty(database) ? "admin" : database,
                    Username = string.IsNullOrEmpty(username) ? "user" : username,
                    Password = string.IsNullOrEmpty(password) ? "p4ssw0rd" : password
                })
                .Build();
        }

        [Fact(Skip = "Vai falhar na esteira")]
        public void Client_Valid_IsNotNull()
        {
            Assert.NotNull(Client);
        }

        public async Task InitializeAsync()
        {
            await Testcontainers.StartAsync();

            Client = new MongoClient(Testcontainers.ConnectionString);
        }

        public Task DisposeAsync()
        {
            return this.Testcontainers.DisposeAsync().AsTask();
        }
    }
}