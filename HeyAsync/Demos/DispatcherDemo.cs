using System.Windows;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DispatcherDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 3;
    public string Title => "03 - Dispatcher";

    public DispatcherDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        await Task.Run(async () =>
        {
            await Task.Delay(500, cancellationToken);

            int backgroundThreadId = Environment.CurrentManagedThreadId;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _logger.WriteLine($"Background Thread war: {backgroundThreadId}");
                _logger.WriteLine($"Dispatcher Thread ist: {Environment.CurrentManagedThreadId}");
            });
        }, cancellationToken);

        _logger.WriteLine($"Nach await wieder UI Thread: {Environment.CurrentManagedThreadId}");
    }
}