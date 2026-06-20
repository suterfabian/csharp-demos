using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ExceptionHandlingDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 12;
    public string Title => "12 - Exception Handling";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        try
        {
            await FailingOperationAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.WriteLine($"Fehler gefangen: {ex.Message}");
        }
    }

    private static async Task FailingOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        throw new InvalidOperationException("Demo-Fehler aus async Methode.");
    }

    public string Description =>
        """
        Demonstriert Fehlerbehandlung in asynchronen Methoden.

        Eine async Methode wirft nach einem await eine Exception.
        Die Exception wird beim await wieder ausgelöst und kann mit
        try/catch normal behandelt werden.

        Dadurch bleibt die Fehlerbehandlung auch bei async Code
        gut kontrollierbar.
        """;
}