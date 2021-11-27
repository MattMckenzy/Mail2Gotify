using Mail2Gotify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Mail2Gotify
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            await Host.CreateDefaultBuilder(args)
                .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Mail2GotifyService>()
                        .AddSingleton<GotifyServiceAppProvider>()
                        .AddSingleton<GotifyService>()
                        .AddSingleton<GotifyMessageStore>()
                        .AddSingleton<GotifyUserAuthenticator>()
                        .AddSingleton<FileSystemCaching>()
                        .AddHttpClient<StaticTokenCaller<GotifyServiceAppProvider>>()
                            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                            .AddPolicyHandler(GetRetryPolicy()); ;
                })
                .RunConsoleAsync();
                #pragma warning restore CA1416 // Validate platform compatibility
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5));
        }
    }
}
