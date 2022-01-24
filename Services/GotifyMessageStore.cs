using Mail2Gotify.Models;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mail2Gotify.Services
{
    public class GotifyMessageStore : MessageStore
    {
        private readonly FileSystemCaching _fileSystemCaching;
        private readonly CacheItemProcessingService _cacheItemProcessingService;

        public GotifyMessageStore(FileSystemCaching fileSystemCaching, CacheItemProcessingService cacheItemProcessingService)
        {
            _fileSystemCaching = fileSystemCaching;
            _cacheItemProcessingService = cacheItemProcessingService;
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            await using MemoryStream stream = new();

            SequencePosition position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out ReadOnlyMemory<byte> memory))
            {
                await stream.WriteAsync(memory, cancellationToken);
            }

            stream.Position = 0;

            MimeMessage message = await MimeMessage.LoadAsync(stream, cancellationToken);

            if (context.Properties.TryGetValue("AppKey", out object credential))
            {
                DateTime messageDateTime = DateTime.Now;
                await _fileSystemCaching.SetAsync(messageDateTime.Ticks.ToString(), new CacheItem
                {
                    Key = messageDateTime.Ticks.ToString(),
                    CacheTime = messageDateTime,
                    GotifyMessage =
                        new GotifyMessage
                        {
                            Date = messageDateTime,
                            Priority = int.TryParse(context.Authentication.User.Split("-").Last(), out int priority) ? priority : 5,
                            Title = $"{message.Subject} ({message.To})",
                            Message = message.TextBody
                        },
                    Credential = (string)credential
                }, cancellationToken);

                _ = Task.Run(_cacheItemProcessingService.ProcessCacheItems, cancellationToken);

                return SmtpResponse.Ok;
            }
            else
                return SmtpResponse.AuthenticationFailed;
        }
    }    
}
