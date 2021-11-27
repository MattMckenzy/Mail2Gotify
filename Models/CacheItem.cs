using System;

namespace Mail2Gotify.Models
{
    [Serializable]
    public class CacheItem
    {
        public string Key { get; set; }

        public DateTime CacheTime { get; set; }

        public string Credential { get; set; }

        public GotifyMessage GotifyMessage { get; set; }
    }
}
