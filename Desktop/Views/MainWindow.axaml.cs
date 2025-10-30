using Avalonia.Controls;
using Avalonia.Input;
using Desktop.ViewModels;

namespace Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TabBorder_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is RequestTabViewModel tab)
        {
            if (DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.ActiveTab = tab;
            }
        }
    }
}