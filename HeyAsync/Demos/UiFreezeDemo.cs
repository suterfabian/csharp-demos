using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class UiFreezeDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 18;
    public string Title => "18 - UI Freeze";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine("CPU-Arbeit wird mit Task.Run vom UI-Thread weggenommen.");

        var sum = 0L;
        var endTime = DateTime.UtcNow.AddSeconds(5);

        while (DateTime.UtcNow < endTime)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < 100_000; i++)
            {
                sum += i % 10;
            }
        }
        
        logger.WriteLine($"Berechnung fertig: {sum}");
        
/*
        var result = await Task.Run(() =>
        {
            var sum = 0L;
            var endTime = DateTime.UtcNow.AddSeconds(5);

            while (DateTime.UtcNow < endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();

                for (var i = 0; i < 100_000; i++)
                {
                    sum += i % 10;
                }
            }

            return sum;
        }, cancellationToken);
        
        logger.WriteLine($"Berechnung fertig: {result}");
*/
    }
}