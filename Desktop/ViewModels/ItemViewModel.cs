using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using Desktop.Helpers;
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
    
    public HeaderViewModel()
    {
        KeyDocument.TextChanged += (s, e) =>
        {
            if (Key != KeyDocument.Text)
            {
                Key = KeyDocument.Text;
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
    
    public QueryParamViewModel()
    {
        KeyDocument.TextChanged += (s, e) =>
        {
            if (Key != KeyDocument.Text)
            {
                Key = KeyDocument.Text;
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