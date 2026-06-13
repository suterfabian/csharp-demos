using System.Windows;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DispatcherDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 3;
    public string Title => "03 - Dispatcher";

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
}