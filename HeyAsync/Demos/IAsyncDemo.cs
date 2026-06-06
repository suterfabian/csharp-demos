namespace HeyAsync.Demos;

public interface IAsyncDemo
{
    int Order { get; }
    string Title { get; }
    Task ExecuteAsync();
}