using Mail2Gotify.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class GotifyService
    {
        private readonly IRestServiceCaller _gotifyAppCaller;

        public GotifyService(StaticTokenCaller<GotifyServiceAppProvider> gotifyAppCaller)
        {
            _gotifyAppCaller = gotifyAppCaller;
        }

        public async Task PushMessage(GotifyMessage gotifyMessage, string credential)
        {
            await _gotifyAppCaller.PostRequestAsync<string>("message", content: JsonConvert.SerializeObject(gotifyMessage), credential: credential);
        }
    }
}