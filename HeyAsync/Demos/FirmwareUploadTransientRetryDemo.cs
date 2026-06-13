using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class FirmwareUploadTransientRetryDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private readonly Random _random = new();

    public int SortOrder => 32;
    public string Title => "32 - Firmware Upload transienter Retry";

    public FirmwareUploadTransientRetryDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        byte[] firmware = new byte[1024];
        int chunkSize = 128;
        int totalChunks = firmware.Length / chunkSize;

        for (int chunk = 1; chunk <= totalChunks; chunk++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await SendChunkWithRetryAsync(chunk, totalChunks, cancellationToken);
        }

        _logger.WriteLine("Firmware Upload erfolgreich abgeschlossen.");
    }

    private async Task SendChunkWithRetryAsync(
        int chunk,
        int totalChunks,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await SendChunkAsync(chunk, cancellationToken);

                int percent = chunk * 100 / totalChunks;

                _logger.WriteLine($"Chunk {chunk}/{totalChunks} OK - {percent}%");
                return;
            }
            catch (InvalidOperationException ex)
            {
                _logger.WriteLine($"Chunk {chunk}, Versuch {attempt}: {ex.Message}");

                if (attempt == maxAttempts)
                {
                    throw;
                }

                int delay = attempt * 300;

                _logger.WriteLine($"Retry in {delay} ms.");

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private async Task SendChunkAsync(int chunk, CancellationToken cancellationToken)
    {
        await Task.Delay(150, cancellationToken);

        bool transientError = _random.Next(0, 5) == 0;

        if (transientError)
        {
            throw new InvalidOperationException($"Transienter BLE-Fehler bei Chunk {chunk}.");
        }
    }
}