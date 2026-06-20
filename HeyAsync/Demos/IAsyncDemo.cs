using HeyAsync.Models;

namespace HeyAsync.Demos;

public interface IAsyncDemo
{
    int SortOrder { get; }
    
    string Title { get; }
    
    string Description { get; }
    
    DemoType Type { get; }
    
    Task ExecuteAsync(CancellationToken cancellationToken);
}