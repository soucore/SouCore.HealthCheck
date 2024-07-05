using AutoFixture;

using Confluent.Kafka;
using Confluent.Kafka.Admin;

using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using System.Net;

using Xunit;

using static Confluent.Kafka.ConfigPropertyNames;

namespace Soucore.HealthCheck.Test.Dependencies
{
    public class KafkaContainerDependency : IAsyncLifetime
    {
        public readonly TestcontainerMessageBroker Testcontainers;
        public readonly TestcontainersContainer zookeeper;
        public IConsumer<Ignore, string> Consumer;
        public string Topic { get; set; }
        private readonly Fixture _fixture;

        public KafkaContainerDependency(
            string username = "",
            string password = "",
            string topic = "healthcheck",
            int port = 9092,
            int zookeeperPort = 2181)
        {
            _fixture = new Fixture();

            Topic = topic;

            var zookeperName = $"zookeeper-{_fixture.Create<Guid>()}";

            var network = new DockerNetwork(_fixture.Create<Guid>().ToString(), "myNetwork");

            zookeeper = new TestcontainersBuilder<TestcontainersContainer>()
                .WithName(zookeperName)
                .WithImage("confluentinc/cp-zookeeper:latest")
                .WithPortBinding(zookeeperPort)
                .WithExposedPort(zookeeperPort)
                .WithEnvironment(new Dictionary<string, string>()
                {
                    {"ZOOKEEPER_CLIENT_PORT", zookeeperPort.ToString()},
                    {"ZOOKEEPER_TICK_TIME", 2000.ToString()},

                })
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(zookeeperPort))
                .Build();

            Testcontainers = new TestcontainersBuilder<KafkaTestcontainer>()
                .WithPortBinding(port)
                .WithExposedPort(port)
                .WithName($"kafka-{_fixture.Create<Guid>()}")
                .WithKafka(new KafkaTestcontainerConfiguration()
                {
                    Username = string.IsNullOrEmpty(username) ? "user" : username,
                    Password = string.IsNullOrEmpty(password) ? "p4ssw0rd" : password
                })
                .WithEnvironment(new Dictionary<string, string>()
                {
                    {"KAFKA_ZOOKEEPER_CONNECT", $"host.docker.internal:{zookeeperPort}"}

                })
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(port))
                .Build();
        }

        [Fact(Skip = "Vai falhar na esteira")]
        public void Client_Valid_IsNotNull()
        {
            Assert.NotNull(Consumer);
        }

        public async Task InitializeAsync()
        {
            await zookeeper.StartAsync();
            await Testcontainers.StartAsync();

            var config = new ConsumerConfig
            {
                BootstrapServers = $"localhost:{Testcontainers.Port}",
                GroupId = "healthcheck-kafka",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true
            };

            Consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            Consumer.Subscribe(Topic);
        }

        public Task DisposeAsync()
        {
            return this.Testcontainers.DisposeAsync().AsTask();
        }

        public async Task CreateTopicAsync(string name = "")
        {
            var config = new ProducerConfig
            {
                BootstrapServers = $"localhost:{Testcontainers.Port}"
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                await producer.ProduceAsync(Topic, new Message<Null, string>() { Value = _fixture.Create<string>() });
            }
        }
    }
}