using Mail2Gotify.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class FileSystemCaching
    {
        public DirectoryInfo CacheDirectory { get; }

        public FileSystemCaching(IConfiguration configuration)
        {
            CacheDirectory = new DirectoryInfo(configuration["Services:Mail2Gotify:CacheDirectory"]);
        }

        public CacheItem Get(string key)
        {
            FileInfo cacheFile = CacheDirectory.GetFiles(KeyParse(key)).FirstOrDefault();

            if (cacheFile != null && cacheFile.Exists)
                return JsonConvert.DeserializeObject<CacheItem>(File.ReadAllText(cacheFile.FullName));
            else
                throw new ArgumentException($"The key: ({key}) was not found in the cache repository.");
        }

        public async Task<CacheItem> GetAsync(string key, CancellationToken token = default)
        {
            FileInfo cacheFile = CacheDirectory.GetFiles($"{KeyParse(key)}.cache").FirstOrDefault();

            if (cacheFile != null && cacheFile.Exists)
                return JsonConvert.DeserializeObject<CacheItem>(await File.ReadAllTextAsync(cacheFile.FullName, token));
            else
                throw new ArgumentException($"The key: ({key}) was not found in the cache repository.");
        }

        public void Remove(string key)
        {
            FileInfo cacheFile = CacheDirectory.GetFiles($"{KeyParse(key)}.cache").FirstOrDefault();

            if (cacheFile != null && cacheFile.Exists)
                File.Delete(cacheFile.FullName);
            else
                throw new ArgumentException($"The key: ({key}) was not found in the cache repository.");

            return;
        }

        public Task RemoveAsync(string key)
        {
            FileInfo cacheFile = CacheDirectory.GetFiles($"{KeyParse(key)}.cache").FirstOrDefault();

            if (cacheFile != null && cacheFile.Exists)
                File.Delete(cacheFile.FullName);
            else
                throw new ArgumentException($"The key: ({key}) was not found in the cache repository.");

            return Task.CompletedTask;
        }

        public void Set(string key, CacheItem cacheItem)
        {
            File.WriteAllText(Path.Combine(CacheDirectory.FullName, $"{KeyParse(key)}.cache"), JsonConvert.SerializeObject(cacheItem));

            return;
        }

        public  async Task SetAsync(string key, CacheItem cacheItem, CancellationToken token = default)
        {
            await File.WriteAllTextAsync(Path.Combine(CacheDirectory.FullName, $"{KeyParse(key)}.cache"), JsonConvert.SerializeObject(cacheItem), token);

            return;
        }

        private static string KeyParse(string key) =>
            new(key.Where(character => char.IsLetterOrDigit(character)).ToArray());
    }
}
