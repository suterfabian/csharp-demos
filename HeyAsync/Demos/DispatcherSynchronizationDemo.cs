using System.Windows;
using HeyAsync.Services;

namespace HeyAsync.Demos;

/// <summary>
/// Demonstriert die Synchronisation zurück auf den WPF-UI-Thread
/// mithilfe des Dispatchers.
///
/// Die eigentliche Arbeit wird in einem Hintergrund-Task ausgeführt.
/// Nach der asynchronen Wartezeit wird der Dispatcher verwendet,
/// um den UI-Zugriff wieder sicher auf dem UI-Thread auszuführen.
///
/// Dieses Muster ist in WPF notwendig, wenn Code aus einem
/// Hintergrund-Thread UI-Elemente aktualisieren soll.
/// </summary>
public sealed class DispatcherSynchronizationDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 33;
    public string Title => "33 - Synchronisation mit Dispatcher";

    // Dispatcher = WPF-spezifisch
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        await Task.Run(async () =>
        {
            await Task.Delay(1000, cancellationToken);

            Application.Current.Dispatcher.Invoke(() =>
            {
                logger.WriteLine($"UI-Update via Dispatcher. Thread: {Environment.CurrentManagedThreadId}");
            });
        }, cancellationToken);
    }
}