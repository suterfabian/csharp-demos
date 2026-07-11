using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RxDistinctUntilChangedDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 41;
    public string Title => "41 - Rx DistinctUntilChanged";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var input = new Subject<string>();

        using var subscription = input
            .DistinctUntilChanged()
            .Subscribe(value => logger.WriteLine($"Neuer Wert: {value}"));

        foreach (var value in new[] { "A", "A", "B", "B", "B", "C" })
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.WriteLine($"Eingabe: {value}");
            input.OnNext(value);

            await Task.Delay(200, cancellationToken);
        }

        input.OnCompleted();
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator DistinctUntilChanged.

        Direkt aufeinanderfolgende gleiche Werte werden ignoriert.
        Nur wenn sich der Wert gegenüber dem vorherigen Wert ändert,
        wird er weitergegeben.

        Das ist nützlich, um unnötige Aktualisierungen zu vermeiden.
        """;
}