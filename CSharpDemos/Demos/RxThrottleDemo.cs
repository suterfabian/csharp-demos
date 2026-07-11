using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RxThrottleDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 40;
    public string Title => "40 - Rx Throttle";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var input = new Subject<string>();

        using var subscription = input
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Subscribe(value => logger.WriteLine($"Verarbeitet nach Ruhezeit: {value}"));

        for (var i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.WriteLine($"Eingabe: {i}");
            input.OnNext($"Text {i}");

            await Task.Delay(200, cancellationToken);
        }

        await Task.Delay(1000, cancellationToken);

        input.OnCompleted();
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator Throttle.

        Eingaben werden erst weitergegeben, wenn für eine bestimmte
        Zeit keine neue Eingabe mehr kommt.

        Dadurch wird bei vielen schnellen Eingaben nur der letzte Wert
        nach der Ruhezeit verarbeitet.
        """;
}