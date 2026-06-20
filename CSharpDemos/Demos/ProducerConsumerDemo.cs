using System.Collections.Concurrent;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class ProducerConsumerDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 19;
    public string Title => "19 - Producer Consumer";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var queue = new BlockingCollection<int>();

        await RunProducerConsumerAsync(queue, cancellationToken);
    }

    private async Task RunProducerConsumerAsync(BlockingCollection<int> queue, CancellationToken cancellationToken)
    {
        var producer = ProduceAsync(queue, cancellationToken);
        var consumer = ConsumeAsync(queue, cancellationToken);

        await Task.WhenAll(producer, consumer);
    }

    private async Task ProduceAsync(BlockingCollection<int> queue, CancellationToken cancellationToken)
    {
        try
        {
            for (var i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger.WriteLine($"Produziert: {i}");
                queue.Add(i, cancellationToken);

                await Task.Delay(200, cancellationToken);
            }
        }
        finally
        {
            queue.CompleteAdding();
        }
    }

    private Task ConsumeAsync(BlockingCollection<int> queue, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            foreach (var item in queue.GetConsumingEnumerable(cancellationToken))
            {
                logger.WriteLine($"Verarbeitet: {item}");
            }
        }, cancellationToken);
    }
    
    public string Description =>
        """
        Demonstriert das Producer-Consumer-Muster.

        Der Producer erzeugt Werte und legt sie in eine Queue.
        Der Consumer liest die Werte aus der Queue und verarbeitet sie.

        BlockingCollection koordiniert dabei das Warten auf neue Elemente
        und das Beenden der Verarbeitung.
        """;
}