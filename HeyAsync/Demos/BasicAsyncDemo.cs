using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class BasicAsyncDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 1;
    public string Title => "01 - Basic async/await";

    public BasicAsyncDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("Start");
        _logger.WriteLine($"Vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(2000);

        _logger.WriteLine($"Nach await - Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("Ende");
    }
}