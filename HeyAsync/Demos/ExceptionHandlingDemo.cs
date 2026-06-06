using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ExceptionHandlingDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 12;
    public string Title => "12 - Exception Handling";

    public ExceptionHandlingDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        try
        {
            await FailingOperationAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.WriteLine($"Fehler gefangen: {ex.Message}");
        }
    }

    private static async Task FailingOperationAsync()
    {
        await Task.Delay(300);
        throw new InvalidOperationException("Demo-Fehler aus async Methode.");
    }
}