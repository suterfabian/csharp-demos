using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ConfigureAwaitDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 10;
    public string Title => "10 - ConfigureAwait";

    public ConfigureAwaitDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine($"Vor await: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500);

        _logger.WriteLine($"Nach normalem await: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500).ConfigureAwait(true);

        _logger.WriteLine($"Nach ConfigureAwait(true): {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("In WPF bleibt man normalerweise auf dem UI-Kontext.");
    }
}