using System.Reactive.Linq;
using System.Reactive.Subjects;
using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RxSelectDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 36;
    public string Title => "36 - Rx Select / Map";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var numbers = new Subject<int>();

        using var subscription = numbers
            .Select(value => value * 10)
            .Subscribe(value => logger.WriteLine($"Ausgabe: {value}"));

        for (var i = 1; i <= 3; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.WriteLine($"Eingabe: {i}");
            numbers.OnNext(i);

            await Task.Delay(300, cancellationToken);
        }

        numbers.OnCompleted();
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator Select.

        Eingehende Werte eines Streams werden transformiert.
        In diesem Beispiel wird jede Zahl mit 10 multipliziert.

        Select entspricht dem Map-Prinzip:
        Eingabe rein, umgewandelter Wert raus.
        """;
}