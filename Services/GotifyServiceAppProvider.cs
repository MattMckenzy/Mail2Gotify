using Microsoft.Extensions.Configuration;
using System;

namespace Mail2Gotify.Services
{
    public class GotifyServiceAppProvider : IRestServiceProvider
    {
        private readonly IConfiguration _configuration;

        public GotifyServiceAppProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetHeader() =>
            _configuration["Services:Gotify:Header"];

        public Uri GetServiceUri() =>
            new(_configuration["Services:Gotify:ServiceUri"]);
    }
}