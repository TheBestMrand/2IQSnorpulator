using Avalonia.Controls;
using Avalonia.Input;
using Desktop.ViewModels;

namespace Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Add Ctrl+S keyboard shortcut
        KeyDown += MainWindow_KeyDown;
    }

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+S to save
        if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control)
        {
            if (DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.SaveActiveRequestCommand.Execute(null);
                e.Handled = true;
            }
        }
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

    private void CollectionHeader_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is CollectionItemViewModel collection)
        {
            if (DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.ToggleCollectionCommand.Execute(collection);
            }
        }
    }

    private void RequestItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is RequestItemViewModel request)
        {
            if (DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.OpenRequestCommand.Execute(request);
            }
        }
    }

    private void HistoryItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is HistoryItemViewModel historyItem)
        {
            if (DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.OpenHistoryEntryCommand.Execute(historyItem);
            }
        }
    }

    private void SnippetItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is Desktop.Models.CodeSnippet snippet)
        {
            if (DataContext is MainWindowViewModel mainViewModel && mainViewModel.ActiveTab != null)
            {
                mainViewModel.ActiveTab.InsertSnippetCommand.Execute(snippet);
            }
        }
    }

    private void HeaderKeyEditor_PreviewKeyDown(object? sender, KeyEventArgs e)
    {
        // Make Tab change focus instead of inserting tab character
        if (e.Key == Key.Tab)
        {
            if (sender is AvaloniaEdit.TextEditor editor)
            {
                // Move focus to next control
                var nextControl = KeyboardNavigationHandler.GetNext(editor, NavigationDirection.Next);
                if (nextControl != null)
                {
                    nextControl.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}