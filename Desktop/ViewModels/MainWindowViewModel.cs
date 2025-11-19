using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Services;
using Data.Models;
using Data.Repositories;
using Environment = Data.Models.Environment;

namespace Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly GreatRequestExecutor _executor;
    private readonly CollectionService _collectionService;
    private readonly EnvironmentDbRepository _environmentRepo;

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

    [ObservableProperty]
    private string _newCollectionName = "";

    [ObservableProperty]
    private bool _isCreatingCollection = false;

    [ObservableProperty]
    private ObservableCollection<Environment> _environments = new();

    [ObservableProperty]
    private Environment? _selectedEnvironment;

    [ObservableProperty]
    private bool _isManagingEnvironments = false;

    [ObservableProperty]
    private string _newEnvironmentName = "";

    [ObservableProperty]
    private bool _isSaveDialogOpen = false;

    [ObservableProperty]
    private string _saveDialogRequestName = "";

    [ObservableProperty]
    private CollectionItemViewModel? _saveDialogSelectedCollection;

    [ObservableProperty]
    private string _saveDialogNewCollectionName = "";

    private RequestTabViewModel? _pendingSaveTab;

    [ObservableProperty]
    private bool _isEnvironmentEditorOpen = false;

    [ObservableProperty]
    private Environment? _editingEnvironment;

    [ObservableProperty]
    private ObservableCollection<EnvironmentVariableViewModel> _environmentVariables = new();

    public bool IsCollectionsTabSelected => SelectedSidebarTab == "Collections";

    public MainWindowViewModel(GreatRequestExecutor executor, CollectionService collectionService, EnvironmentDbRepository environmentRepo)
    {
        _executor = executor;
        _collectionService = collectionService;
        _environmentRepo = environmentRepo;
        
        // Defer data loading until after window is shown (faster startup)
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _ = Task.Run(LoadDataAsync);
        }, Avalonia.Threading.DispatcherPriority.Background);
        
        AddNewTab();
    }

    // ============ TAB MANAGEMENT ============
    
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
        // Check if tab has unsaved changes
        if (!tab.IsSaved && !string.IsNullOrWhiteSpace(tab.Url))
        {
            IsCloseConfirmDialogOpen = true;
            _pendingCloseTab = tab;
            return;
        }

        CloseTabInternal(tab);
    }

    private void CloseTabInternal(RequestTabViewModel tab)
    {
        var index = Tabs.IndexOf(tab);
        Tabs.Remove(tab);

        if (ActiveTab == tab && Tabs.Count > 0)
        {
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

    [ObservableProperty]
    private bool _isCloseConfirmDialogOpen = false;

    private RequestTabViewModel? _pendingCloseTab;
    
    private bool _shouldCloseAfterSave = false;

    [RelayCommand]
    private void ConfirmCloseTab()
    {
        if (_pendingCloseTab != null)
        {
            CloseTabInternal(_pendingCloseTab);
            _pendingCloseTab = null;
        }
        IsCloseConfirmDialogOpen = false;
    }

    [RelayCommand]
    private async Task SaveAndCloseTab()
    {
        if (_pendingCloseTab != null)
        {
            _shouldCloseAfterSave = true;
            ActiveTab = _pendingCloseTab;
            await SaveActiveRequestCommand.ExecuteAsync(null);
            // Don't close yet - will close after save completes
        }
        IsCloseConfirmDialogOpen = false;
    }

    [RelayCommand]
    private void CancelCloseTab()
    {
        _pendingCloseTab = null;
        IsCloseConfirmDialogOpen = false;
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

    // ============ SIDEBAR MANAGEMENT ============
    
    [RelayCommand]
    private void SelectSidebarTab(string tab)
    {
        SelectedSidebarTab = tab;
        OnPropertyChanged(nameof(IsCollectionsTabSelected));
    }

    // ============ COLLECTION MANAGEMENT ============
    
    [RelayCommand]
    private async Task CreateCollection()
    {
        if (string.IsNullOrWhiteSpace(NewCollectionName))
            return;

        await Task.Run(() =>
        {
            var collection = _collectionService.CreateCollection(NewCollectionName);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Collections.Add(new CollectionItemViewModel
                {
                    Id = collection.Id,
                    Name = collection.Name,
                    RequestCount = 0,
                    Requests = new ObservableCollection<RequestItemViewModel>()
                });
                
                NewCollectionName = "";
                IsCreatingCollection = false;
            });
        });
    }

    [RelayCommand]
    private void StartCreatingCollection()
    {
        IsCreatingCollection = true;
        NewCollectionName = "";
    }

    [RelayCommand]
    private void CancelCreatingCollection()
    {
        IsCreatingCollection = false;
        NewCollectionName = "";
    }

    [RelayCommand]
    private async Task DeleteCollection(CollectionItemViewModel collection)
    {
        await Task.Run(() =>
        {
            _collectionService.DeleteCollection(collection.Id);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Collections.Remove(collection);
            });
        });
    }

    [RelayCommand]
    private async Task RenameCollection(CollectionItemViewModel collection)
    {
        // For now, this will be a simple prompt - can be enhanced with a dialog
        var newName = collection.Name + " (renamed)"; // Placeholder
        
        await Task.Run(() =>
        {
            if (_collectionService.RenameCollection(collection.Id, newName))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    collection.Name = newName;
                });
            }
        });
    }

    [RelayCommand]
    private void ToggleCollection(CollectionItemViewModel collection)
    {
        collection.IsExpanded = !collection.IsExpanded;
        
        // Lazy load requests when expanding
        if (collection.IsExpanded && collection.Requests.Count == 0)
        {
            _ = Task.Run(() => LoadCollectionRequests(collection));
        }
    }

    // ============ REQUEST MANAGEMENT ============
    
    [RelayCommand]
    private async Task SaveActiveRequest()
    {
        if (ActiveTab == null)
            return;

        // If no collection selected, prompt user to choose or create one
        if (!ActiveTab.CollectionId.HasValue)
        {
            // Show save dialog with smart naming
            IsSaveDialogOpen = true;
            SaveDialogRequestName = GenerateSmartRequestName(ActiveTab);
            SaveDialogSelectedCollection = Collections.FirstOrDefault(); // Pre-select first collection
            _pendingSaveTab = ActiveTab;
            return;
        }

        var collection = Collections.FirstOrDefault(c => c.Id == ActiveTab.CollectionId.Value);
        if (collection != null)
        {
            await SaveRequestInternal(ActiveTab, collection);
            
            // Close tab if this came from close confirmation
            if (_shouldCloseAfterSave && _pendingCloseTab != null)
            {
                CloseTabInternal(_pendingCloseTab);
                _pendingCloseTab = null;
                _shouldCloseAfterSave = false;
            }
        }
    }

    [RelayCommand]
    private async Task OpenRequest(RequestItemViewModel requestVm)
    {
        await Task.Run(() =>
        {
            var request = _collectionService.GetRequestById(requestVm.Id);
            if (request == null) return;

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                // Check if already open
                var existingTab = Tabs.FirstOrDefault(t => t.RequestId == request.Id);
                if (existingTab != null)
                {
                    foreach (var tab in Tabs)
                        tab.IsActive = false;
                    existingTab.IsActive = true;
                    ActiveTab = existingTab;
                    return;
                }

                // Create new tab and load request
                var newTab = new RequestTabViewModel(_executor);
                newTab.LoadFromRequest(request);

                foreach (var tab in Tabs)
                    tab.IsActive = false;

                Tabs.Add(newTab);
                newTab.IsActive = true;
                ActiveTab = newTab;
            });
        });
    }

    [RelayCommand]
    private async Task DeleteRequest(RequestItemViewModel requestVm)
    {
        await Task.Run(() =>
        {
            _collectionService.DeleteRequest(requestVm.Id);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var collection = Collections.FirstOrDefault(c => c.Id == requestVm.CollectionId);
                if (collection != null)
                {
                    collection.Requests.Remove(requestVm);
                    collection.RequestCount = collection.Requests.Count; // Use actual count instead of decrementing
                }

                // Close tab if open
                var openTab = Tabs.FirstOrDefault(t => t.RequestId == requestVm.Id);
                if (openTab != null)
                {
                    CloseTab(openTab);
                }
            });
        });
    }

    [RelayCommand]
    private async Task AddRequestToCollection(CollectionItemViewModel collection)
    {
        await Task.Run(() =>
        {
            var request = _collectionService.CreateRequest("New Request", collection.Id);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var requestVm = new RequestItemViewModel
                {
                    Id = request.Id,
                    Name = request.Name,
                    Method = request.Method,
                    CollectionId = request.CollectionId
                };
                
                collection.Requests.Add(requestVm);
                collection.RequestCount++;
                
                // Open the new request in a tab
                _ = OpenRequestCommand.ExecuteAsync(requestVm);
            });
        });
    }

    // ============ HISTORY MANAGEMENT ============
    
    [RelayCommand]
    private async Task OpenHistoryEntry(HistoryItemViewModel historyVm)
    {
        await Task.Run(() =>
        {
            var historyEntry = _collectionService.GetRecentHistory()
                .FirstOrDefault(h => h.Id == historyVm.Id);
            
            if (historyEntry == null) return;

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var newTab = new RequestTabViewModel(_executor);
                newTab.LoadFromHistory(historyEntry);

                foreach (var tab in Tabs)
                    tab.IsActive = false;

                Tabs.Add(newTab);
                newTab.IsActive = true;
                ActiveTab = newTab;
            });
        });
    }

    [RelayCommand]
    private async Task ClearHistory()
    {
        await Task.Run(() =>
        {
            _collectionService.ClearHistory();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                History.Clear();
            });
        });
    }

    [RelayCommand]
    private async Task DeleteHistoryEntry(HistoryItemViewModel historyVm)
    {
        await Task.Run(() =>
        {
            _collectionService.DeleteHistoryEntry(historyVm.Id);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                History.Remove(historyVm);
            });
        });
    }

    [RelayCommand]
    private async Task RefreshHistory()
    {
        await Task.Run(() => LoadHistoryAsync());
    }

    // ============ DATA LOADING ============
    
    private async Task LoadDataAsync()
    {
        await LoadCollectionsAsync();
        await LoadHistoryAsync();
        await LoadEnvironmentsAsync();
    }

    private async Task LoadCollectionsAsync()
    {
        await Task.Run(() =>
        {
            var collections = _collectionService.GetAllCollections().ToList();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Collections.Clear();
                foreach (var collection in collections)
                {
                    Collections.Add(new CollectionItemViewModel
                    {
                        Id = collection.Id,
                        Name = collection.Name,
                        RequestCount = collection.Requests.Count,
                        IsExpanded = false
                    });
                }
            });
        });
    }

    private async Task LoadCollectionRequests(CollectionItemViewModel collectionVm)
    {
        await Task.Run(() =>
        {
            var requests = _collectionService.GetRequestsByCollectionId(collectionVm.Id).ToList();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                collectionVm.Requests.Clear();
                foreach (var request in requests)
                {
                    collectionVm.Requests.Add(new RequestItemViewModel
                    {
                        Id = request.Id,
                        Name = request.Name,
                        Method = request.Method,
                        CollectionId = request.CollectionId
                    });
                }
            });
        });
    }

    private async Task LoadHistoryAsync()
    {
        await Task.Run(() =>
        {
            var history = _collectionService.GetRecentHistory(50).ToList();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                History.Clear();
                foreach (var entry in history)
                {
                    History.Add(new HistoryItemViewModel
                    {
                        Id = entry.Id,
                        Name = entry.Request.Name,
                        Method = entry.Request.Method,
                        Url = entry.Request.Url,
                        ExecutedAt = entry.ExecutedAt
                    });
                }
            });
        });
    }

    // ============ ENVIRONMENT MANAGEMENT ============

    private async Task LoadEnvironmentsAsync()
    {
        await Task.Run(() =>
        {
            var environments = _environmentRepo.GetAll().ToList();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Environments.Clear();
                foreach (var env in environments)
                {
                    Environments.Add(env);
                }
                
                // Select first environment or none
                SelectedEnvironment = Environments.FirstOrDefault();
            });
        });
    }

    [RelayCommand]
    private void ToggleManageEnvironments()
    {
        IsManagingEnvironments = !IsManagingEnvironments;
    }

    [RelayCommand]
    private async Task CreateEnvironment()
    {
        if (string.IsNullOrWhiteSpace(NewEnvironmentName))
            return;

        await Task.Run(() =>
        {
            var env = new Environment { Name = NewEnvironmentName };
            env.Id = _environmentRepo.Insert(env);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Environments.Add(env);
                NewEnvironmentName = "";
                IsManagingEnvironments = false;
                SelectedEnvironment = env;
            });
        });
    }

    [RelayCommand]
    private async Task DeleteEnvironment(Environment env)
    {
        await Task.Run(() =>
        {
            _environmentRepo.Delete(env.Id);
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Environments.Remove(env);
                if (SelectedEnvironment == env)
                {
                    SelectedEnvironment = Environments.FirstOrDefault();
                }
            });
        });
    }

    [RelayCommand]
    private void EditEnvironmentVariables(Environment env)
    {
        EditingEnvironment = env;
        EnvironmentVariables.Clear();
        
        foreach (var kvp in env.Variables)
        {
            EnvironmentVariables.Add(new EnvironmentVariableViewModel
            {
                Key = kvp.Key,
                Value = kvp.Value
            });
        }
        
        // Add empty row for new variable
        EnvironmentVariables.Add(new EnvironmentVariableViewModel());
        
        IsEnvironmentEditorOpen = true;
    }

    [RelayCommand]
    private void AddEnvironmentVariable()
    {
        EnvironmentVariables.Add(new EnvironmentVariableViewModel());
    }

    [RelayCommand]
    private void RemoveEnvironmentVariable(EnvironmentVariableViewModel variable)
    {
        EnvironmentVariables.Remove(variable);
    }

    [RelayCommand]
    private async Task SaveEnvironmentVariables()
    {
        if (EditingEnvironment == null) return;

        await Task.Run(() =>
        {
            EditingEnvironment.Variables.Clear();
            foreach (var variable in EnvironmentVariables.Where(v => !string.IsNullOrWhiteSpace(v.Key)))
            {
                EditingEnvironment.Variables[variable.Key] = variable.Value;
            }
            
            _environmentRepo.Update(EditingEnvironment);
        });

        IsEnvironmentEditorOpen = false;
        EditingEnvironment = null;
    }

    [RelayCommand]
    private void CancelEnvironmentEdit()
    {
        IsEnvironmentEditorOpen = false;
        EditingEnvironment = null;
        EnvironmentVariables.Clear();
    }

    // ============ SAVE DIALOG ============

    [RelayCommand]
    private async Task SaveToSelectedCollection()
    {
        if (_pendingSaveTab == null) return;

        // Validation: Need either a selected collection or a new collection name
        if (SaveDialogSelectedCollection == null && string.IsNullOrWhiteSpace(SaveDialogNewCollectionName))
        {
            // Could show error message here
            return;
        }

        // Validation: Need a request name
        if (string.IsNullOrWhiteSpace(SaveDialogRequestName))
        {
            SaveDialogRequestName = GenerateSmartRequestName(_pendingSaveTab);
        }

        CollectionItemViewModel? targetCollection = SaveDialogSelectedCollection;

        // Create new collection if specified
        if (!string.IsNullOrWhiteSpace(SaveDialogNewCollectionName))
        {
            await Task.Run(() =>
            {
                var collection = _collectionService.CreateCollection(SaveDialogNewCollectionName);
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    targetCollection = new CollectionItemViewModel
                    {
                        Id = collection.Id,
                        Name = collection.Name,
                        RequestCount = 0,
                        Requests = new ObservableCollection<RequestItemViewModel>()
                    };
                    Collections.Insert(0, targetCollection);
                });
            });
            
            await Task.Delay(100); // Wait for UI update
        }

        if (targetCollection != null)
        {
            _pendingSaveTab.CollectionId = targetCollection.Id;
            _pendingSaveTab.Name = SaveDialogRequestName;
            IsSaveDialogOpen = false;
            
            // Now save
            await SaveRequestInternal(_pendingSaveTab, targetCollection);
            
            // Close tab if this came from close confirmation
            if (_shouldCloseAfterSave && _pendingCloseTab != null)
            {
                CloseTabInternal(_pendingCloseTab);
                _pendingCloseTab = null;
                _shouldCloseAfterSave = false;
            }
            
            _pendingSaveTab = null;
            SaveDialogNewCollectionName = "";
        }
    }

    [RelayCommand]
    private void CancelSaveDialog()
    {
        IsSaveDialogOpen = false;
        _pendingSaveTab = null;
        SaveDialogNewCollectionName = "";
        SaveDialogRequestName = "";
    }

    private string GenerateSmartRequestName(RequestTabViewModel tab)
    {
        // If already has a name that's not "New Request", use it
        if (!string.IsNullOrWhiteSpace(tab.Name) && tab.Name != "New Request")
        {
            return tab.Name;
        }

        // Generate smart name from URL
        if (!string.IsNullOrWhiteSpace(tab.Url))
        {
            try
            {
                var uri = new Uri(tab.Url);
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                if (pathSegments.Length > 0)
                {
                    // Use last path segment (e.g., "users" from "/api/users")
                    var lastSegment = pathSegments.Last();
                    return $"{tab.ActualMethod} {lastSegment}";
                }
                else
                {
                    // Just domain (e.g., "GET example.com")
                    return $"{tab.ActualMethod} {uri.Host}";
                }
            }
            catch
            {
                // If URL parsing fails, just use method
                return $"{tab.ActualMethod} Request";
            }
        }

        return "New Request";
    }

    private async Task SaveRequestInternal(RequestTabViewModel tab, CollectionItemViewModel collection)
    {
        // Read text from documents on UI thread (required for AvaloniaEdit)
        string bodyText = tab.BodyDocument.Text;
        string scriptText = tab.ScriptDocument.Text;
        string postScriptText = tab.PostScriptDocument.Text;
        
        await Task.Run(() =>
        {
            var headers = tab.Headers
                .Where(h => !string.IsNullOrWhiteSpace(h.Key))
                .ToDictionary(h => h.Key, h => h.Value);
            var query = tab.QueryParams
                .Where(q => !string.IsNullOrWhiteSpace(q.Key))
                .ToDictionary(q => q.Key, q => q.Value);

            var request = _collectionService.SaveRequestFromTab(
                tab.Name,
                tab.ActualMethod,
                tab.Url,
                bodyText,
                tab.GetContentType(),
                headers,
                query,
                scriptText,
                postScriptText,
                tab.ScriptLanguage == "C#" ? Data.Models.Enums.Languages.Csharp : Data.Models.Enums.Languages.Python,
                collection.Id,
                tab.RequestId
            );

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                tab.RequestId = request.Id;
                tab.IsSaved = true;

                var existingRequest = collection.Requests.FirstOrDefault(r => r.Id == request.Id);
                if (existingRequest == null)
                {
                    collection.Requests.Add(new RequestItemViewModel
                    {
                        Id = request.Id,
                        Name = request.Name,
                        Method = request.Method,
                        CollectionId = request.CollectionId
                    });
                    collection.RequestCount++;
                }
                else
                {
                    existingRequest.Name = request.Name;
                    existingRequest.Method = request.Method;
                }
            });
        });
    }
}

public partial class EnvironmentVariableViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _key = "";

    [ObservableProperty]
    private string _value = "";
}
