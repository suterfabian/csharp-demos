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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        using BlockingCollection<int> queue = new();

        Task producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                queue.Add(i, cancellationToken);
                _logger.WriteLine($"Produziert: {i}");

                await Task.Delay(200, cancellationToken);
            }

            queue.CompleteAdding();
        }, cancellationToken);

        Task consumer = Task.Run(() =>
        {
            foreach (int item in queue.GetConsumingEnumerable(cancellationToken))
            {
                _logger.WriteLine($"Verarbeitet: {item}");
            }
        }, cancellationToken);

        await Task.WhenAll(producer, consumer);
    }
}