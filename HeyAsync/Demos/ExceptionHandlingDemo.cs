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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        try
        {
            await FailingOperationAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.WriteLine($"Fehler gefangen: {ex.Message}");
        }
    }

    private static async Task FailingOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        throw new InvalidOperationException("Demo-Fehler aus async Methode.");
    }
}