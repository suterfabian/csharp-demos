using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ExceptionHandlingDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 12;
    public string Title => "12 - Exception Handling";

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
}