using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class FirmwareUploadSimulationDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int SortOrder => 31;
    public string Title => "31 - Firmware Upload Simulation";

    public FirmwareUploadSimulationDemo(IUiLogger logger)
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

            await Task.Delay(250, cancellationToken);

            int percent = chunk * 100 / totalChunks;

            _logger.WriteLine($"Chunk {chunk}/{totalChunks} gesendet - {percent}%");
        }

        _logger.WriteLine("Firmware Upload abgeschlossen.");
    }
}