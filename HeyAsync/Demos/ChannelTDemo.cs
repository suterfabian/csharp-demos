using System.Threading.Channels;
using HeyAsync.Services;

namespace HeyAsync.Demos;

/*
    ChannelWriter<T>
        schreibt Werte in den Channel

    ChannelReader<T>
        liest Werte aus dem Channel

    writer.TryComplete()
        signalisiert: Es kommen keine weiteren Werte

    ReadAllAsync(...)
        liest so lange, bis der Channel abgeschlossen ist
 */

public sealed class ChannelTDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 35;
    public string Title => "35 - Channel<T>";

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
}
