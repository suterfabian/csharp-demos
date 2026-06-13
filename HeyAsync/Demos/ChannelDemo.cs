using System.Threading.Channels;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ChannelDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 20;
    public string Title => "20 - Channel";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var channel = Channel.CreateUnbounded<int>();

        var producer = Task.Run(async () =>
        {
            for (var i = 1; i <= 5; i++)
            {
                await channel.Writer.WriteAsync(i, cancellationToken);
                logger.WriteLine($"Gesendet: {i}");
                await Task.Delay(200, cancellationToken);
            }

            channel.Writer.Complete();
        }, cancellationToken);

        var consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
            {
                logger.WriteLine($"Empfangen: {item}");
            }
        }, cancellationToken);

        await Task.WhenAll(producer, consumer);
    }
}