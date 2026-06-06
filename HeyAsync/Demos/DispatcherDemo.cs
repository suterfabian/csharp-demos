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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        int backgroundThreadId = await Task.Run(() =>
        {
            return Environment.CurrentManagedThreadId;
        });

        _logger.WriteLine($"Background Thread: {backgroundThreadId}");
        _logger.WriteLine($"Zurück auf UI Thread: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("Nach await läuft der Code wieder auf dem UI Thread.");
    }
}