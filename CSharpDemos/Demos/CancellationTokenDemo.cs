using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class CancellationTokenDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 8;
    public string Title => "08 - CancellationToken";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using CancellationTokenSource demoTimeout = new(TimeSpan.FromSeconds(5));
        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, demoTimeout.Token);

        for (var i = 1; i <= 10; i++)
        {
            linkedCts.Token.ThrowIfCancellationRequested();

            logger.WriteLine($"Schritt {i}");
            await Task.Delay(1000, linkedCts.Token);
        }
    }

    public string Description =>
        """
        Demonstriert den Abbruch einer asynchronen Operation.

        Die Demo verwendet einen externen CancellationToken und zusätzlich
        einen internen Timeout von 5 Sekunden.

        Mit CreateLinkedTokenSource werden beide Abbruchsignale kombiniert.
        Die Schleife endet, sobald einer der beiden Tokens abgebrochen wird.
        """;
}