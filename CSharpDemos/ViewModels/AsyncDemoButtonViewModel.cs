using System.Windows.Input;

namespace CSharpDemos.ViewModels;

public sealed class AsyncDemoButtonViewModel
{
    public string Title { get; }
    public ICommand Command { get; }
    public string Description { get; }

    public AsyncDemoButtonViewModel(string title, ICommand command, string description)
    {
        Title = title;
        Command = command;
        Description = description;
    }
}