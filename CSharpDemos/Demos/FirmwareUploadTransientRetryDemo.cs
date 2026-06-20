using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class FirmwareUploadTransientRetryDemo(IUiLogger logger) : IAsyncDemo
{
    private readonly Random _random = new();

    public int SortOrder => 32;
    public string Title => "32 - Firmware Upload transienter Retry";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var firmware = new byte[1024];
        const int chunkSize = 128;
        var totalChunks = firmware.Length / chunkSize;

        for (var chunk = 1; chunk <= totalChunks; chunk++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await SendChunkWithRetryAsync(chunk, totalChunks, cancellationToken);
        }

        logger.WriteLine("Firmware Upload erfolgreich abgeschlossen.");
    }

    private async Task SendChunkWithRetryAsync(
        int chunk,
        int totalChunks,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await SendChunkAsync(chunk, cancellationToken);

                var percent = chunk * 100 / totalChunks;

                logger.WriteLine($"Chunk {chunk}/{totalChunks} OK - {percent}%");
                return;
            }
            catch (InvalidOperationException ex)
            {
                logger.WriteLine($"Chunk {chunk}, Versuch {attempt}: {ex.Message}");

                if (attempt == maxAttempts)
                {
                    throw;
                }

                var delay = attempt * 300;

                logger.WriteLine($"Retry in {delay} ms.");

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private async Task SendChunkAsync(int chunk, CancellationToken cancellationToken)
    {
        await Task.Delay(150, cancellationToken);

        var transientError = _random.Next(0, 5) == 0;

        if (transientError)
        {
            throw new InvalidOperationException($"Transienter BLE-Fehler bei Chunk {chunk}.");
        }
    }

    public string Description =>
        """
        Simuliert einen Firmware-Upload mit transienten Fehlern.

        Jeder Chunk wird einzeln gesendet. Bei einem kurzfristigen Fehler
        wird der Versand desselben Chunks erneut versucht.

        Nach mehreren fehlgeschlagenen Versuchen wird der Fehler weitergegeben.
        """;
}