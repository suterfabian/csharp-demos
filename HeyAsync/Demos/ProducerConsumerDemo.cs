using System.Collections.Concurrent;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ProducerConsumerDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 19;
    public string Title => "19 - Producer Consumer";

    public ProducerConsumerDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        using BlockingCollection<int> queue = new();

        Task producer = Task.Run(() =>
        {
            for (int i = 1; i <= 5; i++)
            {
                queue.Add(i);
                _logger.WriteLine($"Produziert: {i}");
            }

            queue.CompleteAdding();
        });

        Task consumer = Task.Run(() =>
        {
            foreach (int item in queue.GetConsumingEnumerable())
            {
                _logger.WriteLine($"Verarbeitet: {item}");
            }
        });

        await Task.WhenAll(producer, consumer);
    }
}