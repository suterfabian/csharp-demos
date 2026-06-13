namespace HeyAsync.Demos;

public interface IAsyncDemo
{
    int SortOrder { get; }
    string Title { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}