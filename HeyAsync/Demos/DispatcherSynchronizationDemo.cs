using System.Windows;
using HeyAsync.Services;

namespace HeyAsync.Demos;

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
            await Task.Delay(3000, cancellationToken);

            Application.Current.Dispatcher.Invoke(() =>
            {
                logger.WriteLine($"UI-Update via Dispatcher. Thread: {Environment.CurrentManagedThreadId}");
            });
        }, cancellationToken);
    }
}