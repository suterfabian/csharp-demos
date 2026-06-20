namespace CSharpDemos.Services;

public interface IUiLogger
{
    string OutputText { get; }

    event EventHandler? OutputChanged;

    void WriteLine(string text);
    void WriteHeader(string text);
    void Clear();
}