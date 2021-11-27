using Mail2Gotify.Exceptions;
using Mail2Gotify.Models;
using Microsoft.Extensions.Logging;
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

        public GotifyMessageStore(FileSystemCaching fileSystemCaching)
        {
            _fileSystemCaching = fileSystemCaching;
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
                            Title = message.Subject,
                            Message = message.TextBody
                        },
                    Credential = (string)credential
                }, cancellationToken);

                return SmtpResponse.Ok;
            }
            else
                return SmtpResponse.AuthenticationFailed;
        }
    }    
}
