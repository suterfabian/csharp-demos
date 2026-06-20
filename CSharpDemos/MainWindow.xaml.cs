using System.Windows;
using CSharpDemos.ViewModels;

namespace CSharpDemos;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}