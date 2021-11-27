using SmtpServer;
using SmtpServer.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class GotifyUserAuthenticator : IUserAuthenticator
    {
        public Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken token)
        {
            context.Properties.Add("AppKey", password);

            return Task.FromResult(password.Length == 15);
        }
    }
}
