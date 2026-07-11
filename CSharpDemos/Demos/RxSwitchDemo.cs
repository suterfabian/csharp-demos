using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RxSwitchDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 38;
    public string Title => "38 - Rx Switch";
    public DemoType Type => DemoType.Rx;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using var searchText = new Subject<string>();

        using var subscription = searchText
            .Select(text => SearchAsync(text, cancellationToken).ToObservable())
            .Switch()
            .Subscribe(result => logger.WriteLine($"Ergebnis: {result}"));

        foreach (var text in new[] { "M", "Ma", "Max" })
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.WriteLine($"Sucheingabe: {text}");
            searchText.OnNext(text);

            await Task.Delay(300, cancellationToken);
        }

        await Task.Delay(1500, cancellationToken);

        searchText.OnCompleted();
    }

    private async Task<string> SearchAsync(string text, CancellationToken cancellationToken)
    {
        logger.WriteLine($"Starte Suche für: {text}");

        await Task.Delay(1000, cancellationToken);

        return $"Treffer für {text}";
    }
    
    public string Description =>
        """
        Demonstriert den Rx-Operator Switch.

        Jede Sucheingabe startet eine neue asynchrone Suche.
        Sobald eine neue Suche startet, wird nur noch deren Ergebnis
        berücksichtigt.

        Ältere Suchergebnisse werden ignoriert.
        """;
}