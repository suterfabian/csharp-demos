using HeyAsync.Services;

namespace HeyAsync.Demos;

/// <summary>
/// Führt eine einfache Throttle-Demo aus.
///
/// Es werden 10 Events im Abstand von 200 ms erzeugt.
/// Die eigentliche Verarbeitung darf jedoch höchstens einmal
/// innerhalb von 700 ms ausgeführt werden.
///
/// Liegt die letzte Ausführung mindestens 700 ms zurück,
/// wird das aktuelle Event verarbeitet.
/// Andernfalls wird das Event übersprungen.
///
/// Dadurch wird verhindert, dass eine häufig ausgelöste Aktion
/// zu oft ausgeführt wird.
/// </summary>
public sealed class ThrottleDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 28;
    public string Title => "28 - Throttle";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var lastExecution = DateTime.MinValue;

        for (var i = 1; i <= 10; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var now = DateTime.Now;

            if (now - lastExecution >= TimeSpan.FromMilliseconds(700))
            {
                lastExecution = now;
                logger.WriteLine($"Ausgeführt: Event {i}");
            }
            else
            {
                logger.WriteLine($"Übersprungen: Event {i}");
            }

            await Task.Delay(200, cancellationToken);
        }
    }
}