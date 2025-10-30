using CommunityToolkit.Mvvm.ComponentModel;

namespace Desktop.ViewModels;

public partial class CollectionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private int _requestCount;
}

public partial class HistoryItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _method = "GET";

    [ObservableProperty]
    private string _url = "";

    public string MethodColor => Method switch
    {
        "GET" => "#1a4d2e",
        "POST" => "#4d3a1a",
        "PUT" => "#1a3a4d",
        "DELETE" => "#4d1a1a",
        "PATCH" => "#4d1a4d",
        _ => "#2a2a2a"
    };

    public string MethodTextColor => Method switch
    {
        "GET" => "#4ade80",
        "POST" => "#fbbf24",
        "PUT" => "#60a5fa",
        "DELETE" => "#f87171",
        "PATCH" => "#c084fc",
        _ => "#888888"
    };

    partial void OnMethodChanged(string value)
    {
        OnPropertyChanged(nameof(MethodColor));
        OnPropertyChanged(nameof(MethodTextColor));
    }
}

public partial class HeaderViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _key = "";

    [ObservableProperty]
    private string _value = "";
}

public partial class QueryParamViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _key = "";

    [ObservableProperty]
    private string _value = "";
}