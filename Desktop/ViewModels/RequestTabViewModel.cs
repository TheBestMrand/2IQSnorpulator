using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Services;
using Data.Models;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using Desktop.Helpers;

namespace Desktop.ViewModels;

public partial class RequestTabViewModel : ViewModelBase
{
    private readonly GreatRequestExecutor _executor;

    [ObservableProperty]
    private string _name = "New Request";

    [ObservableProperty]
    private string _method = "GET";

    [ObservableProperty]
    private string _customMethod = "";

    [ObservableProperty]
    private bool _isCustomMethod = false;

    [ObservableProperty]
    private string _url = "";

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private string _selectedOptionTab = "Headers";

    [ObservableProperty]
    private string _selectedResponseTab = "Body";

    [ObservableProperty]
    private ObservableCollection<HeaderViewModel> _headers = new();

    [ObservableProperty]
    private ObservableCollection<QueryParamViewModel> _queryParams = new();

    // Use TextDocument for body editor with syntax highlighting
    [ObservableProperty]
    private TextDocument _bodyDocument = new();

    [ObservableProperty]
    private string _bodyType = "JSON";
    
    [ObservableProperty]
    private IHighlightingDefinition? _bodySyntaxHighlighting;
    
    [ObservableProperty]
    private string _jsonError = "";
    
    [ObservableProperty]
    private bool _hasJsonError = false;

    [ObservableProperty]
    private string _preRequestScript = "";

    [ObservableProperty]
    private string _postRequestScript = "";

    [ObservableProperty]
    private string _scriptLanguage = "C#";

    [ObservableProperty]
    private string _authType = "No Auth";

    [ObservableProperty]
    private string _authToken = "";

    [ObservableProperty]
    private string _authUsername = "";

    [ObservableProperty]
    private string _authPassword = "";

    // Use TextDocument for efficient large text handling - no truncation needed!
    [ObservableProperty]
    private TextDocument _responseDocument = new();
    
    [ObservableProperty]
    private IHighlightingDefinition? _responseSyntaxHighlighting;

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _responseHeaders = new();

    [ObservableProperty]
    private string _responseCookies = "No cookies";

    [ObservableProperty]
    private string _responseStatus = "";

    [ObservableProperty]
    private int _responseTime;

    [ObservableProperty]
    private string _responseSize = "";

    [ObservableProperty]
    private bool _isSending = false;

    public string ActualMethod => IsCustomMethod ? CustomMethod : Method;

    public string MethodColor => ActualMethod switch
    {
        "GET" => "#1a4d2e",
        "POST" => "#4d3a1a",
        "PUT" => "#1a3a4d",
        "DELETE" => "#4d1a1a",
        "PATCH" => "#4d1a4d",
        _ => "#2a2a2a"
    };

    public string MethodTextColor => ActualMethod switch
    {
        "GET" => "#4ade80",
        "POST" => "#fbbf24",
        "PUT" => "#60a5fa",
        "DELETE" => "#f87171",
        "PATCH" => "#c084fc",
        _ => "#888888"
    };

    public string ResponseStatusColor => ResponseStatus.StartsWith("2") ? "#1a4d2e" :
                                         ResponseStatus.StartsWith("4") ? "#4d3a1a" :
                                         ResponseStatus.StartsWith("5") ? "#4d1a1a" : "#2a2a2a";

    public string ResponseStatusTextColor => ResponseStatus.StartsWith("2") ? "#4ade80" :
                                             ResponseStatus.StartsWith("4") ? "#fbbf24" :
                                             ResponseStatus.StartsWith("5") ? "#f87171" : "#888888";

    public RequestTabViewModel(GreatRequestExecutor executor)
    {
        _executor = executor;
        InitializeMockData();
        
        // Validate JSON whenever body text changes
        BodyDocument.TextChanged += (s, e) => ValidateJson();
    }

    [RelayCommand]
    private void SelectOptionTab(string tab)
    {
        SelectedOptionTab = tab;
    }

    [RelayCommand]
    private void SelectResponseTab(string tab)
    {
        SelectedResponseTab = tab;
    }

    [RelayCommand]
    private void AddHeader()
    {
        Headers.Add(new HeaderViewModel
        {
            Key = "",
            Value = ""
        });
    }

    [RelayCommand]
    private void RemoveHeader(HeaderViewModel header)
    {
        Headers.Remove(header);
    }

    [RelayCommand]
    private void AddQueryParam()
    {
        QueryParams.Add(new QueryParamViewModel
        {
            Key = "",
            Value = ""
        });
    }

    [RelayCommand]
    private void RemoveQueryParam(QueryParamViewModel param)
    {
        QueryParams.Remove(param);
    }

    [RelayCommand]
    private void ResetToStandardMethod()
    {
        IsCustomMethod = false;
        Method = "GET";
        CustomMethod = "";
        OnPropertyChanged(nameof(ActualMethod));
        OnPropertyChanged(nameof(MethodColor));
        OnPropertyChanged(nameof(MethodTextColor));
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            ResponseStatus = "Error";
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                ResponseDocument.Text = "Please enter a valid URL";
            });
            return;
        }

        IsSending = true;
        
        // Capture values to avoid cross-thread access
        var methodToUse = ActualMethod;
        var urlToUse = Url;
        var bodyToUse = (BodyType == "None" || string.IsNullOrWhiteSpace(BodyDocument.Text)) ? null : BodyDocument.Text;
        var bodyTypeToUse = GetContentType();
        var headersToUse = Headers
            .Where(h => !string.IsNullOrWhiteSpace(h.Key))
            .ToDictionary(h => h.Key, h => h.Value);
        var queryToUse = QueryParams
            .Where(q => !string.IsNullOrWhiteSpace(q.Key))
            .ToDictionary(q => q.Key, q => q.Value);
        var scriptToUse = PreRequestScript;
        var postScriptToUse = PostRequestScript;
        var scriptLangToUse = ScriptLanguage;

        try
        {
            // Auto-update Content-Type header
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                UpdateContentTypeHeader();
            });

            // Execute everything on background thread
            var result = await Task.Run(async () =>
            {
                var request = new Request
                {
                    Method = methodToUse,
                    Url = urlToUse,
                    Body = bodyToUse,
                    BodyType = bodyTypeToUse,
                    Headers = headersToUse,
                    Query = queryToUse,
                    PreRequestScript = scriptToUse,
                    PostResponseScript = postScriptToUse,
                    ScriptLanguage = scriptLangToUse == "C#" 
                        ? Data.Models.Enums.Languages.Csharp 
                        : Data.Models.Enums.Languages.Python
                };

                var resp = await _executor.ExecuteRequestAsync(request).ConfigureAwait(false);
                
                // Format on background thread
                string formattedBody;
                
                if (resp.BodyType?.Contains("json") == true && !string.IsNullOrWhiteSpace(resp.Body))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(resp.Body);
                        formattedBody = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
                    }
                    catch
                    {
                        formattedBody = resp.Body ?? "";
                    }
                }
                else
                {
                    formattedBody = resp.Body ?? "";
                }

                return (resp, formattedBody);
            }).ConfigureAwait(false);

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                var (response, formattedBody) = result;
                
                ResponseStatus = response.StatusCode == 0 
                    ? "Error" 
                    : $"{response.StatusCode} {GetStatusText(response.StatusCode)}";
                ResponseTime = (int)response.ResponseTime.TotalMilliseconds;
                ResponseSize = FormatSize(response.Size);
                
                // Update response headers
                ResponseHeaders.Clear();
                foreach (var header in response.Headers)
                {
                    ResponseHeaders.Add(new KeyValuePair<string, string>(header.Key, header.Value));
                }

                // Update cookies
                if (response.Headers.TryGetValue("Set-Cookie", out var cookies))
                {
                    ResponseCookies = cookies;
                }
                else
                {
                    ResponseCookies = "No cookies";
                }
                
                // Just set the full response - TextEditor handles large text efficiently!
                ResponseDocument.Text = formattedBody;
                
                // Set syntax highlighting with BRIGHT, readable colors
                if (response.BodyType?.Contains("json") == true)
                {
                    ResponseSyntaxHighlighting = JsonHighlightingHelper.GetJsonHighlighting();
                }
                else if (response.BodyType?.Contains("xml") == true)
                {
                    ResponseSyntaxHighlighting = JsonHighlightingHelper.GetXmlHighlighting();
                }
                else
                {
                    ResponseSyntaxHighlighting = null;
                }
                
                OnPropertyChanged(nameof(ResponseStatusColor));
                OnPropertyChanged(nameof(ResponseStatusTextColor));
            });
        }
        catch (Exception ex)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                ResponseStatus = "Error";
                ResponseDocument.Text = $"Error: {ex.Message}";
                ResponseTime = 0;
                ResponseSize = "";
                ResponseHeaders.Clear();
                ResponseCookies = "No cookies";
                
                OnPropertyChanged(nameof(ResponseStatusColor));
                OnPropertyChanged(nameof(ResponseStatusTextColor));
            });
        }
        finally
        {
            IsSending = false;
        }
    }

    private void UpdateContentTypeHeader()
    {
        var contentType = GetContentType();
        
        var existingHeader = Headers.FirstOrDefault(h => 
            h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        
        if (BodyType == "None" || string.IsNullOrWhiteSpace(contentType))
        {
            if (existingHeader != null)
            {
                Headers.Remove(existingHeader);
            }
            return;
        }
        
        if (existingHeader != null)
        {
            existingHeader.Value = contentType;
        }
        else
        {
            var emptyHeader = Headers.FirstOrDefault(h => string.IsNullOrWhiteSpace(h.Key));
            if (emptyHeader != null)
            {
                emptyHeader.Key = "Content-Type";
                emptyHeader.Value = contentType;
            }
            else
            {
                Headers.Insert(0, new HeaderViewModel
                {
                    Key = "Content-Type",
                    Value = contentType
                });
            }
        }
    }

    private string GetContentType()
    {
        return BodyType switch
        {
            "None" => "",
            "JSON" => "application/json",
            "XML" => "application/xml",
            "Form Data" => "application/x-www-form-urlencoded",
            "Raw" => "text/plain",
            _ => "application/json"
        };
    }

    private string GetStatusText(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Internal Server Error",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            _ => ""
        };
    }

    private string FormatSize(int bytes)
    {
        if (bytes < 1024) return $"Size: {bytes} B";
        if (bytes < 1024 * 1024) return $"Size: {bytes / 1024.0:F1} KB";
        return $"Size: {bytes / (1024.0 * 1024.0):F1} MB";
    }

    partial void OnMethodChanged(string value)
    {
        if (value == "Custom...")
        {
            IsCustomMethod = true;
            CustomMethod = "";
        }
        OnPropertyChanged(nameof(ActualMethod));
        OnPropertyChanged(nameof(MethodColor));
        OnPropertyChanged(nameof(MethodTextColor));
    }

    partial void OnCustomMethodChanged(string value)
    {
        OnPropertyChanged(nameof(ActualMethod));
        OnPropertyChanged(nameof(MethodColor));
        OnPropertyChanged(nameof(MethodTextColor));
    }

    partial void OnResponseStatusChanged(string value)
    {
        OnPropertyChanged(nameof(ResponseStatusColor));
        OnPropertyChanged(nameof(ResponseStatusTextColor));
    }

    partial void OnBodyTypeChanged(string value)
    {
        UpdateContentTypeHeader();
        UpdateBodySyntaxHighlighting();
        ValidateJson();
    }
    
    private void UpdateBodySyntaxHighlighting()
    {
        BodySyntaxHighlighting = BodyType switch
        {
            "JSON" => JsonHighlightingHelper.GetJsonHighlighting(),
            "XML" => JsonHighlightingHelper.GetXmlHighlighting(),
            _ => null
        };
    }
    
    private void ValidateJson()
    {
        if (BodyType != "JSON" || string.IsNullOrWhiteSpace(BodyDocument.Text))
        {
            HasJsonError = false;
            JsonError = "";
            return;
        }
        
        try
        {
            JsonDocument.Parse(BodyDocument.Text);
            HasJsonError = false;
            JsonError = "";
        }
        catch (JsonException ex)
        {
            HasJsonError = true;
            JsonError = $"JSON Error: {ex.Message}";
        }
    }

    private void InitializeMockData()
    {
        Headers.Add(new HeaderViewModel
        {
            Key = "Content-Type",
            Value = "application/json"
        });
        Headers.Add(new HeaderViewModel
        {
            Key = "",
            Value = ""
        });

        QueryParams.Add(new QueryParamViewModel
        {
            Key = "",
            Value = ""
        });

        PreRequestScript = "";  // Empty by default
        PostRequestScript = "";  // Empty by default

        Url = "https://api.github.com/users/octocat";
        
        // Initialize syntax highlighting for JSON
        UpdateBodySyntaxHighlighting();
    }
}