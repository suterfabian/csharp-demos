using System.Windows;
using HeyAsync.ViewModels;

namespace HeyAsync;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}