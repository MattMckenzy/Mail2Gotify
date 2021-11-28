using Mail2Gotify.Exceptions;
using Mail2Gotify.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class CacheItemProcessingService
    {
        private readonly FileSystemCaching _fileSystemCaching;
        private readonly GotifyService _gotifyService;
        private readonly ILogger<CacheItemProcessingService> _logger;

        public CacheItemProcessingService(FileSystemCaching fileSystemCaching, GotifyService gotifyService, ILogger<CacheItemProcessingService> logger)
        {
            _fileSystemCaching = fileSystemCaching;
            _gotifyService = gotifyService;
            _logger = logger;
        }

        public async Task ProcessCacheItems()
        {
            foreach (FileInfo fileInfo in _fileSystemCaching.CacheDirectory.GetFiles("*.cache").ToArray())
            {
                if (fileInfo != null && fileInfo.Exists)
                {
                    try
                    {
                        CacheItem cacheItem = await _fileSystemCaching.GetAsync(Path.GetFileNameWithoutExtension(fileInfo.Name));
                        await _gotifyService.PushMessage(cacheItem.GotifyMessage, cacheItem.Credential);
                        await _fileSystemCaching.RemoveAsync(cacheItem.Key);
                    }
                    catch (CommunicationException communicationException)
                    {
                        _logger.Log(LogLevel.Error, $"Could not communicate with Gotify: {communicationException}");
                    }
                }
            }
        }
    }
}
