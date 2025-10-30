using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Services;

namespace Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly GreatRequestExecutor _executor;

    [ObservableProperty]
    private ObservableCollection<RequestTabViewModel> _tabs = new();

    [ObservableProperty]
    private RequestTabViewModel? _activeTab;

    [ObservableProperty]
    private ObservableCollection<CollectionItemViewModel> _collections = new();

    [ObservableProperty]
    private ObservableCollection<HistoryItemViewModel> _history = new();

    [ObservableProperty]
    private string _selectedSidebarTab = "Collections";

    public bool IsCollectionsTabSelected => SelectedSidebarTab == "Collections";

    public MainWindowViewModel(GreatRequestExecutor executor)
    {
        _executor = executor;
        InitializeMockData();
        AddNewTab();
    }

    [RelayCommand]
    private void AddNewTab()
    {
        var newTab = new RequestTabViewModel(_executor)
        {
            Name = "New Request",
            Method = "GET",
            Url = "",
            IsActive = false
        };

        // Deactivate all tabs
        foreach (var tab in Tabs)
        {
            tab.IsActive = false;
        }

        Tabs.Add(newTab);
        newTab.IsActive = true;
        ActiveTab = newTab;
    }

    [RelayCommand]
    private void CloseTab(RequestTabViewModel tab)
    {
        var index = Tabs.IndexOf(tab);
        Tabs.Remove(tab);

        if (ActiveTab == tab && Tabs.Count > 0)
        {
            // Activate adjacent tab
            var newIndex = index > 0 ? index - 1 : 0;
            if (newIndex < Tabs.Count)
            {
                Tabs[newIndex].IsActive = true;
                ActiveTab = Tabs[newIndex];
            }
        }
        else if (Tabs.Count == 0)
        {
            ActiveTab = null;
        }
    }

    [RelayCommand]
    private void SelectSidebarTab(string tab)
    {
        SelectedSidebarTab = tab;
        OnPropertyChanged(nameof(IsCollectionsTabSelected));
    }

    partial void OnActiveTabChanged(RequestTabViewModel? value)
    {
        if (value != null)
        {
            foreach (var tab in Tabs)
            {
                tab.IsActive = tab == value;
            }
        }
    }

    private void InitializeMockData()
    {
        // Mock Collections
        Collections.Add(new CollectionItemViewModel
        {
            Name = "Authentication API",
            RequestCount = 12
        });
        Collections.Add(new CollectionItemViewModel
        {
            Name = "User Management",
            RequestCount = 8
        });
        Collections.Add(new CollectionItemViewModel
        {
            Name = "Payment Gateway",
            RequestCount = 15
        });
        Collections.Add(new CollectionItemViewModel
        {
            Name = "Data Analytics",
            RequestCount = 6
        });

        // Mock History
        History.Add(new HistoryItemViewModel
        {
            Name = "Get User Profile",
            Method = "GET",
            Url = "https://api.example.com/users/123"
        });
        History.Add(new HistoryItemViewModel
        {
            Name = "Create Payment",
            Method = "POST",
            Url = "https://api.example.com/payments"
        });
        History.Add(new HistoryItemViewModel
        {
            Name = "Update Settings",
            Method = "PUT",
            Url = "https://api.example.com/settings"
        });
        History.Add(new HistoryItemViewModel
        {
            Name = "Delete Session",
            Method = "DELETE",
            Url = "https://api.example.com/sessions/abc123"
        });
    }
}