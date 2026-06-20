using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class SimpleRetryDemo(IUiLogger logger) : IAsyncDemo
{
    private int _unstableOperationCallCount;

    public int SortOrder => 22;
    public string Title => "22 - Retry einfach";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        _unstableOperationCallCount = 0;

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await UnstableOperationAsync(cancellationToken);
                logger.WriteLine("Operation erfolgreich.");
                return;
            }
            catch (InvalidOperationException ex)
            {
                logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
            }
        }

        logger.WriteLine("Alle Versuche fehlgeschlagen.");
    }

    private async Task UnstableOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(300, cancellationToken);

        _unstableOperationCallCount++;

        if (_unstableOperationCallCount < 3)
        {
            throw new InvalidOperationException("Transienter Fehler.");
        }
    }
    
    public string Description =>
        """
        Demonstriert einen einfachen Retry-Mechanismus.

        Eine instabile Operation wird mehrfach versucht.
        Schlägt sie fehl, wird der Fehler protokolliert und der
        nächste Versuch gestartet.

        Nach der maximalen Anzahl Versuche gilt die Operation
        als fehlgeschlagen.
        """;
}