using System.Windows;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class DispatcherDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 3;
    public string Title => "03 - Dispatcher";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        await Task.Run(async () =>
        {
            await Task.Delay(3000, cancellationToken);

            var backgroundThreadId = Environment.CurrentManagedThreadId;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                logger.WriteLine($"Background Thread war: {backgroundThreadId}");
                logger.WriteLine($"Dispatcher Thread ist: {Environment.CurrentManagedThreadId}");
            });
        }, cancellationToken);

        logger.WriteLine($"Nach await wieder UI Thread: {Environment.CurrentManagedThreadId}");
    }

    public string Description =>
        """
        Demonstriert den Wechsel zwischen Hintergrund-Thread und UI-Thread.

        Die Arbeit läuft zuerst in Task.Run auf einem Hintergrund-Thread.
        Mit dem Dispatcher wird anschliessend sicher zurück auf den
        WPF-UI-Thread gewechselt.

        Das ist notwendig, wenn aus Hintergrund-Code UI-Elemente
        aktualisiert werden sollen.
        """;
}