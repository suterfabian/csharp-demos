using HeyAsync.Services;

namespace HeyAsync.Demos;

/*
    Diese Demo zeigt TaskCompletionSource<T>.

    TaskCompletionSource<T> wird verwendet,
    wenn ein Task nicht direkt durch eine async-Methode entsteht,
    sondern durch ein externes Ereignis abgeschlossen werden soll.

    Beispiele:
        - Callback wird ausgelöst
        - Event wird empfangen
        - Signal kommt von außen
        - alte API soll in async/await integriert werden

    In dieser Demo:
        - tcs.Task wird awaited
        - ein Hintergrundtask setzt später das Resultat
        - Cancellation kann den Task abbrechen

    Dadurch kann normal mit await gearbeitet werden,
    obwohl das Ergebnis manuell von außen gesetzt wird.
*/
public sealed class TaskCompletionSourceDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 25;
    public string Title => "25 - TaskCompletionSource";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        TaskCompletionSource<string> tcs = new();

        await using var registration =
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        _ = Task.Run(async () =>
        {
            await Task.Delay(1000, cancellationToken);
            tcs.TrySetResult("Signal empfangen.");
        }, cancellationToken);

        logger.WriteLine("Warte auf externes Signal...");

        var result = await tcs.Task;

        logger.WriteLine(result);
    }
}