using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.MongoDb
{
    public class MongoDbHealthCheck : IHealthCheckCustom
    {
        private static readonly ConcurrentDictionary<string, MongoClient> _mongoClient 
            = new ConcurrentDictionary<string, MongoClient>();
        private readonly ILogger<MongoDbHealthCheck> _logger;
        private readonly MongoDbHealthCheckSettings _settings;
        private readonly string _specifiedDatabase;
        private readonly MongoClientSettings _mongoClientSettings;

        public bool Disabled { get; set; }

        public MongoDbHealthCheck(ILogger<MongoDbHealthCheck> logger, MongoDbHealthCheckSettings settings)
        {
            _logger = logger;
            _settings = settings;
            Disabled = _settings.Disable;

            if (!string.IsNullOrWhiteSpace(_settings.ConnectionString))
            {
                var mongoUrl = MongoUrl.Create(_settings.ConnectionString);
                _specifiedDatabase = mongoUrl?.DatabaseName;
                _mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            }

            if(_settings.MongoClientSettings != null)
                _mongoClientSettings = _settings.MongoClientSettings;
        }

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Start HealthCheck MongoDb!");

            try
            {
                var mongoClient = _mongoClient.GetOrAdd(_mongoClientSettings.ToString(), _ => new MongoClient(_mongoClientSettings));

                if (!string.IsNullOrEmpty(_specifiedDatabase))
                {
                    using var cursor = await mongoClient
                        .GetDatabase(_specifiedDatabase)
                        .ListCollectionNamesAsync(cancellationToken: stoppingToken);
                    var result = !string.IsNullOrWhiteSpace(cursor.FirstOrDefault(stoppingToken));
                    return new HealthCheckResult(result, Message());
                }
                else
                {
                    using var cursor = await mongoClient.ListDatabaseNamesAsync(stoppingToken);
                    var result = cursor.FirstOrDefault(stoppingToken) != null;
                    return new HealthCheckResult(result, Message());
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "HealthCheck - MongoDb unhealthy");
                return new HealthCheckResult(false, Message(), ex);
            }
        }


        public string Message()
        {
            return string.Concat("Not Connected: ", _settings.ConnectionString);
        }
    }
}
