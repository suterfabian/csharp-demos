using System.Windows.Input;

namespace CSharpDemos.ViewModels;

public sealed class AsyncDemoButtonViewModel
{
    public string Title { get; }
    public ICommand Command { get; }

    public AsyncDemoButtonViewModel(string title, ICommand command)
    {
        Title = title;
        Command = command;
    }
}