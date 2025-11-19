using System;
using System.Collections.ObjectModel;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using Desktop.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Desktop.ViewModels;

public partial class CollectionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private int _requestCount;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private ObservableCollection<RequestItemViewModel> _requests = new();
}

public partial class RequestItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _method = "GET";

    [ObservableProperty]
    private int _collectionId;

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

public partial class HistoryItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _method = "GET";

    [ObservableProperty]
    private string _url = "";

    [ObservableProperty]
    private DateTime _executedAt;

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

    public string TimeAgo
    {
        get
        {
            var span = DateTime.UtcNow - ExecutedAt;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return ExecutedAt.ToLocalTime().ToString("MMM dd");
        }
    }

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
    
    [ObservableProperty]
    private TextDocument _keyDocument = new();

    [ObservableProperty]
    private TextDocument _valueDocument = new();

    [ObservableProperty]
    private IHighlightingDefinition _variableSyntaxHighlighting = SyntaxHighlightingHelper.GetVariableOnlyHighlighting();

    partial void OnKeyChanged(string value)
    {
        if (KeyDocument.Text != value)
        {
            KeyDocument.Text = value;
        }
    }

    partial void OnValueChanged(string value)
    {
        if (ValueDocument.Text != value)
        {
            ValueDocument.Text = value;
        }
    }
    
    public event EventHandler? KeyChanged;

    public HeaderViewModel()
    {
        KeyDocument.TextChanged += (s, e) =>
        {
            if (Key != KeyDocument.Text)
            {
                Key = KeyDocument.Text;
                KeyChanged?.Invoke(this, EventArgs.Empty);
            }
        };

        ValueDocument.TextChanged += (s, e) =>
        {
            if (Value != ValueDocument.Text)
            {
                Value = ValueDocument.Text;
            }
        };
    }
}

public partial class QueryParamViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _key = "";

    [ObservableProperty]
    private string _value = "";
    
    [ObservableProperty]
    private TextDocument _keyDocument = new();

    [ObservableProperty]
    private TextDocument _valueDocument = new();
    
    [ObservableProperty]
    private IHighlightingDefinition _variableSyntaxHighlighting = SyntaxHighlightingHelper.GetVariableOnlyHighlighting();

    partial void OnKeyChanged(string value)
    {
        if (KeyDocument.Text != value)
        {
            KeyDocument.Text = value;
        }
    }

    partial void OnValueChanged(string value)
    {
        if (ValueDocument.Text != value)
        {
            ValueDocument.Text = value;
        }
    }
    
    public event EventHandler? KeyChanged;
    public event EventHandler? ValueChanged;

    public QueryParamViewModel()
    {
        KeyDocument.TextChanged += (s, e) =>
        {
            if (Key != KeyDocument.Text)
            {
                Key = KeyDocument.Text;
                KeyChanged?.Invoke(this, EventArgs.Empty);
            }
        };

        ValueDocument.TextChanged += (s, e) =>
        {
            if (Value != ValueDocument.Text)
            {
                Value = ValueDocument.Text;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        };
    }
}