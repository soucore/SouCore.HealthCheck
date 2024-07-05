using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace Soucore.HealthCheck.Smtp
{
    public class SmtpHealthCheck : IHealthCheckCustom
    {
        private readonly ILogger<SmtpHealthCheck> _logger;
        private readonly SmtpHealthCheckSettings _settings;

        public bool Disabled { get; set; }

        public SmtpHealthCheck(ILogger<SmtpHealthCheck> logger, SmtpHealthCheckSettings settings)
        {
            _logger = logger;
            _settings = settings;
            Disabled = _settings.Disable;
        }

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Start HealthCheck SMTP!");

            if (_settings == null)
                throw new NotImplementedException("SMTP configuration not found.");

            try
            {
                var result = await TestConnection(_settings.Host, _settings.Port);
                return new HealthCheckResult(result, Message());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HealthCheck - Smtp unhealthy");
                return new HealthCheckResult(false, Message(), ex);
            }
        }

        public async Task<bool> TestConnection(string smtpServerAddress, int port)
        {
            var hostEntry = Dns.GetHostEntry(smtpServerAddress);
            var endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
            using var tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _logger.LogInformation("Trying to connect to SMTP server.");
            await tcpSocket.ConnectAsync(endPoint);
            if (!CheckResponse(tcpSocket, 220))
            {
                _logger.LogError("Failed connecting to SMTP server.");
                return false;
            }

            _logger.LogInformation("Trying to initiate session conversation.");
            SendData(tcpSocket, string.Format("HELO {0}\r\n", Dns.GetHostName()));
            if (!CheckResponse(tcpSocket, 250))
            {
                _logger.LogError("Failed initiating session conversation.");
                return false;
            }

            return true;
        }

        private void SendData(Socket socket, string data)
        {
            var dataArray = Encoding.ASCII.GetBytes(data);
            socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }

        private bool CheckResponse(Socket socket, int expectedCode)
        {
            while (socket.Available == 0)
            {
                Thread.Sleep(100);
            }
            var responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            var responseData = Encoding.ASCII.GetString(responseArray);
            var responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }


        private string Message()
        {
            return string.Concat("TestConnection SMP Fail! Host: ", _settings.Host, ":", _settings.Port);
        }
    }
}