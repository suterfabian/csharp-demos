using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ProgressDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 9;
    public string Title => "09 - Progress";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        // IProgress<int> merkt sich beim Erstellen den aktuellen SynchronizationContext.
        IProgress<int> progress = new Progress<int>(value =>
        {
            logger.WriteLine($"Fortschritt: {value}%"); // UI-Zugriff
        });

        await Task.Run(async () =>
        {
            for (var i = 0; i <= 100; i += 20)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(i);
                await Task.Delay(300, cancellationToken);
            }
        }, cancellationToken);

        logger.WriteLine("Fertig.");
    }
}