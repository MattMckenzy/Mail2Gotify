using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.ComponentModel;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class Mail2GotifyService : IHostedService
    {
        private readonly CacheItemProcessingService _cacheItemProcessingService;
        private readonly GotifyMessageStore _gotifyMessageStore;
        private readonly GotifyUserAuthenticator _gotifyUserAuthenticator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Mail2GotifyService> _logger;

        private SmtpServer.SmtpServer _smtpServer;

        public Mail2GotifyService(CacheItemProcessingService cacheItemProcessingService, GotifyMessageStore gotifyMessageStore, GotifyUserAuthenticator gotifyUserAuthenticator, IConfiguration configuration, ILogger<Mail2GotifyService> logger)
        {
            _cacheItemProcessingService = cacheItemProcessingService;
            _gotifyMessageStore = gotifyMessageStore;
            _gotifyUserAuthenticator = gotifyUserAuthenticator;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cacheItemProcessingService.ProcessCacheItems();

            ISmtpServerOptions options = new SmtpServerOptionsBuilder()
              .ServerName(_configuration["Services:Mail2Gotify:HostAddress"])
              .Endpoint(builder =>
                builder                
                .Port(_configuration.GetValue<int>("Services:Mail2Gotify:HostPort"))
                .IsSecure(true)
                .AuthenticationRequired()
                .SupportedSslProtocols(System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12)
                .Certificate(CreateX509Certificate2()))
              .Build();

            ServiceProvider serviceProvider = new();
            serviceProvider.Add(_gotifyMessageStore);
            serviceProvider.Add(_gotifyUserAuthenticator);

            _logger.Log(LogLevel.Information, $"Mail2Gotify server starting!");

            _smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);
            await _smtpServer.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, $"Mail2Gotify server stopping!");

            #pragma warning disable CA2016 // We don't want to cancel waiting, server will receive SIGKILL if taking too long.
            Task.WaitAll(_smtpServer.ShutdownTask);
            #pragma warning restore CA2016 // We don't want to cancel waiting, server will receive SIGKILL if taking too long.

            return Task.CompletedTask;
        }

        public X509Certificate CreateX509Certificate2()
        {
            RSA rsa = RSA.Create();
            CertificateRequest certificateRequest = new($"cn={_configuration["Certificate:Name"]}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            X509Certificate certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            return new X509Certificate2(certificate.Export(X509ContentType.Pfx, _configuration["Certificate:Password"]), _configuration["Certificate:Password"]);
        }
    }
}
