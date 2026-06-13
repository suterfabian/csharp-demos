using System.Runtime.CompilerServices;
using HeyAsync.Services;

namespace HeyAsync.Demos;

/*
    Diese Demo zeigt einen Async Stream mit IAsyncEnumerable<T>.

    Ein Async Stream liefert Werte nicht alle auf einmal,
    sondern nach und nach über die Zeit.

    In dieser Demo:
        - GenerateValuesAsync erzeugt alle 400 ms einen neuen Wert
        - yield return gibt jeden Wert einzeln zurück
        - await foreach verarbeitet die Werte, sobald sie verfügbar sind
        - der CancellationToken erlaubt einen sauberen Abbruch

    Das ist nützlich für:
        - Datenströme
        - Paging
        - Events
        - Sensorwerte
        - fortlaufende Verarbeitung
*/
public sealed class AsyncStreamDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 26;
    public string Title => "26 - Async Stream";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        await foreach (var value in GenerateValuesAsync(cancellationToken))
        {
            logger.WriteLine($"Wert: {value}");
        }
    }

    private static async IAsyncEnumerable<int> GenerateValuesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 1; i <= 5; i++)
        {
            await Task.Delay(400, cancellationToken);
            yield return i;
        }
    }
}