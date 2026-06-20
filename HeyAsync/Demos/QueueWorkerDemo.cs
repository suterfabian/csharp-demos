using System.Threading.Channels;
using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class QueueWorkerDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 29;
    public string Title => "29 - Queue Worker";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var queue = Channel.CreateUnbounded<string>();

        var worker = Task.Run(async () =>
        {
            await foreach (var command in queue.Reader.ReadAllAsync(cancellationToken))
            {
                logger.WriteLine($"Verarbeite: {command}");
                await Task.Delay(100, cancellationToken);
            }
        }, cancellationToken);

        for (var i = 1; i <= 5; i++)
        {
            await queue.Writer.WriteAsync($"Command {i}", cancellationToken);
            logger.WriteLine($">Eingereiht: Command {i}");
            
            await Task.Delay(40, cancellationToken);
        }

        queue.Writer.Complete();

        await worker;

        logger.WriteLine("Queue leer.");
    }

    public string Description =>
        """
        Demonstriert einen einfachen Queue-Worker mit Channel.
        
        Commands werden in eine Queue geschrieben, während ein Worker
        diese im Hintergrund nacheinander ausliest und verarbeitet.
        
        Das Einreihen erfolgt schneller als die Verarbeitung.
        Dadurch wird sichtbar, dass der Channel die Commands puffert
        und der Worker sie kontrolliert der Reihe nach abarbeitet.
        
        Nach dem Einreihen aller Commands wird der Writer abgeschlossen.
        Der Worker verarbeitet anschliessend alle verbleibenden Einträge,
        bis die Queue leer ist.
        """;
}