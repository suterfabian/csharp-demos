using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class LockDemo(IUiLogger logger) : IAsyncDemo
{
    private readonly Lock _syncRoot = new();
    public int SortOrder => 5;
    public string Title => "05 - Lock";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var counter = 0;

        Action<int> loopBody = index =>
        {
            lock (_syncRoot)
            {
                counter++;
                logger.WriteLine($"Index: {index}, Counter: {counter}");
            }
        };

        await Task.Run(() =>
        {
            Parallel.For(0, 1000, new ParallelOptions
            {
                CancellationToken = cancellationToken
            }, loopBody);
        }, cancellationToken);

        logger.WriteLine("Erwartet: 1000");
        logger.WriteLine($"Tatsächlich: {counter}");
        logger.WriteLine("lock schützt den kritischen Bereich.");
    }

    public string Description =>
        """
        Demonstriert den Schutz eines kritischen Bereichs mit lock.

        Mehrere parallele Operationen greifen auf denselben Counter zu.
        Durch lock darf immer nur ein Thread gleichzeitig den Counter erhöhen.

        Dadurch wird ein Race Condition verhindert.
        
        Nicht mit await verwendbar!
        Für asynchronen Code verwendet man stattdessen: SemaphoreSlim
        """;
}