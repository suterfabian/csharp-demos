using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RxCombineLatestDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 37;
    public string Title => "37 - Rx CombineLatest";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var age = new Subject<int>();
        using var name = new Subject<string>();

        using var subscription = name
            .CombineLatest(age, (currentName, currentAge) =>
                $"{currentName} ({currentAge})")
            .Subscribe(value => logger.WriteLine($"Kombiniert: {value}"));

        logger.WriteLine("Age = 20");
        age.OnNext(20);

        await Task.Delay(300, cancellationToken);

        logger.WriteLine("Name = Max");
        name.OnNext("Max");

        await Task.Delay(300, cancellationToken);

        logger.WriteLine("Age = 21");
        age.OnNext(21);

        await Task.Delay(300, cancellationToken);

        logger.WriteLine("Name = Anna");
        name.OnNext("Anna");

        name.OnCompleted();
        age.OnCompleted();
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator CombineLatest.

        Zwei Streams werden kombiniert, sobald beide mindestens
        einen Wert geliefert haben.

        Danach erzeugt jede Änderung eines Streams einen neuen
        kombinierten Wert mit den jeweils letzten Werten beider Streams.
        """;
}