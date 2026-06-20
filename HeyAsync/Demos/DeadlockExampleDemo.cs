using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DeadlockExampleDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 17;
    public string Title => "17 - Deadlock Beispiel";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine("Problematisch in WPF wäre z.B.: SomeAsync().Result");
        logger.WriteLine("Das kann den UI-Thread blockieren.");

        var result = await SomeAsync(cancellationToken);

        logger.WriteLine($"Richtig mit await: {result}");
    }

    private static async Task<string> SomeAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        return "fertig";
    }

    public string Description =>
        """
        Zeigt ein typisches Deadlock-Risiko in UI-Anwendungen.

        Das Blockieren mit .Result oder .Wait() kann den UI-Thread sperren
        und verhindern, dass eine asynchrone Methode fortgesetzt wird.

        Die korrekte Variante ist await, weil der UI-Thread dabei
        nicht blockiert wird.
        """;
}