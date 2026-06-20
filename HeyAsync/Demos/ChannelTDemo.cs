using System.Threading.Channels;
using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ChannelTDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 35;
    public string Title => "35 - Channel<T>";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var channel = Channel.CreateUnbounded<int>();

        var producerTask = ProduceAsync(channel.Writer, cancellationToken);
        var consumerTask = ConsumeAsync(channel.Reader, cancellationToken);

        await Task.WhenAll(producerTask, consumerTask);
    }

    private async Task ProduceAsync(ChannelWriter<int> writer, CancellationToken cancellationToken)
    {
        try
        {
            for (var i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger.WriteLine($"Produziert: {i}");

                await writer.WriteAsync(i, cancellationToken);

                await Task.Delay(150, cancellationToken);
            }
            
            writer.TryComplete();
        }
        catch (OperationCanceledException)
        {
            writer.TryComplete();
            throw;
        }
        catch (Exception ex)
        {
            writer.TryComplete(ex);
            throw;
        }
    }

    private async Task ConsumeAsync(ChannelReader<int> reader, CancellationToken cancellationToken)
    {
        await foreach (var item in reader.ReadAllAsync(cancellationToken))
        {
            logger.WriteLine($"Verarbeitet: {item}");

            await Task.Delay(500, cancellationToken);
        }
    }

    public string Description =>
        """
        Demonstriert einen typisierten Channel mit Channel<T>.
        
        Der Producer schreibt Zahlen in den Channel.
        Der Consumer liest die Zahlen parallel aus und verarbeitet sie langsamer.
        
        Dadurch wird sichtbar, wie der Channel Producer und Consumer
        voneinander entkoppelt und Werte puffert.
        
        ChannelWriter<T>
            schreibt Werte in den Channel
        
        ChannelReader<T>
            liest Werte aus dem Channel
        
        writer.TryComplete()
            signalisiert: Es kommen keine weiteren Werte
        
        ReadAllAsync(...)
            liest so lange, bis der Channel abgeschlossen ist
        """;
}
