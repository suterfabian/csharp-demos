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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine($"Vor await: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500, cancellationToken);

        _logger.WriteLine($"Nach normalem await: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500, cancellationToken).ConfigureAwait(true);

        _logger.WriteLine($"Nach ConfigureAwait(true): {Environment.CurrentManagedThreadId}");
    }
}