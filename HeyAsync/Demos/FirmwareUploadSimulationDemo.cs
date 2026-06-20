using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class FirmwareUploadSimulationDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 31;
    public string Title => "31 - Firmware Upload Simulation";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var firmware = new byte[1024];
        const int chunkSize = 128;
        var totalChunks = firmware.Length / chunkSize;

        for (var chunk = 1; chunk <= totalChunks; chunk++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(250, cancellationToken);

            var percent = chunk * 100 / totalChunks;

            logger.WriteLine($"Chunk {chunk}/{totalChunks} gesendet - {percent}%");
        }

        logger.WriteLine("Firmware Upload abgeschlossen.");
    }
}