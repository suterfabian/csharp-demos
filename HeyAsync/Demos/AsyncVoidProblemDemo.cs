using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncVoidProblemDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 11;
    public string Title => "11 - async void Problem";

    public AsyncVoidProblemDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("async void sollte außerhalb von UI-Events vermieden werden.");
        _logger.WriteLine("Grund: Fehler können nicht sauber awaited oder gefangen werden.");

        await SafeAsyncMethod();

        _logger.WriteLine("Besser: async Task statt async void.");
    }

    private async Task SafeAsyncMethod()
    {
        await Task.Delay(300);
        _logger.WriteLine("Diese Methode kann awaited werden.");
    }
}