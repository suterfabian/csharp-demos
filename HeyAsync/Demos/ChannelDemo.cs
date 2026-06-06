using System.Threading.Channels;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ChannelDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 20;
    public string Title => "20 - Channel";

    public ChannelDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        Channel<int> channel = Channel.CreateUnbounded<int>();

        Task producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                await channel.Writer.WriteAsync(i);
                _logger.WriteLine($"Gesendet: {i}");
            }

            channel.Writer.Complete();
        });

        Task consumer = Task.Run(async () =>
        {
            await foreach (int item in channel.Reader.ReadAllAsync())
            {
                _logger.WriteLine($"Empfangen: {item}");
            }
        });

        await Task.WhenAll(producer, consumer);
    }
}