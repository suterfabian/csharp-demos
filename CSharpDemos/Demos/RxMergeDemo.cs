using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RxMergeDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 39;
    public string Title => "39 - Rx Merge";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var streamA = new Subject<string>();
        using var streamB = new Subject<string>();

        using var subscription = streamA
            .Merge(streamB)
            .Subscribe(value => logger.WriteLine($"Empfangen: {value}"));

        streamA.OnNext("A1");
        await Task.Delay(200, cancellationToken);

        streamB.OnNext("B1");
        await Task.Delay(200, cancellationToken);

        streamA.OnNext("A2");
        await Task.Delay(200, cancellationToken);

        streamB.OnNext("B2");

        streamA.OnCompleted();
        streamB.OnCompleted();
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator Merge.

        Mehrere Streams werden zu einem gemeinsamen Stream zusammengeführt.
        Jeder Wert wird weitergegeben, sobald er in einem der Streams entsteht.

        Die Reihenfolge entspricht dem tatsächlichen Eintreffen der Werte.
        """;
}