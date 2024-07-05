using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.HealthCheck
{
    public class HealthCheckHttpListener
    {
        private readonly HealthCheckResponseConfiguration _config;
        private readonly HttpListener _httpListener = new();
        private Task _workerThread;
        private readonly CancellationToken _stoppingToken;
        private readonly object _syncObject = new();

        public HealthCheckHttpListener(HealthCheckResponseConfiguration config, CancellationToken stoppingToken)
        {
            _config = config;
            _stoppingToken = stoppingToken;
            _httpListener.Prefixes.Add($"http://{config.Hostname}:{config.Port}{config.UrlPath}/");
        }

        public void Start()
        {
            lock (_syncObject)
            {
                _workerThread = Task.Factory.StartNew(WorkerProc, _stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
        
        private void WorkerProc()
        {
            _httpListener.Start();
            try
            {
                while (!_stoppingToken.IsCancellationRequested)
                {
                    var context = _httpListener.GetContext();
                    Task.Run(() => ProcessHealthCheck(context, _stoppingToken));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                _httpListener.Stop();
                _httpListener.Close();
            }
        }

        private async Task ProcessHealthCheck(HttpListenerContext context, CancellationToken stoppingToken)
        {
            try
            {
                var request = context.Request;
                using var response = context.Response;
                if (!request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
                    || !request.Url.PathAndQuery.Equals(_config.UrlPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.StatusCode = 404;
                }

                // var healthCheckResult = await _healthCheck.ExecuteAsync(_config.AliasName, cancellationToken)
                //     .ConfigureAwait(false);

                var healthCheckResult = new JsonHealthCheckResponse(new StatusResponse(true));

                response.ContentType = healthCheckResult.ContentType;
                response.ContentEncoding = healthCheckResult.ContentEncoding;

                response.StatusCode = (int)healthCheckResult.StatusCode;
                byte[] data = Encoding.UTF8.GetBytes(healthCheckResult.Body);

                response.ContentLength64 = data.LongLength;
                await response.OutputStream.WriteAsync(data, 0, data.Length, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            } 
        }
    }
}