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
using Desktop.Models;

namespace Desktop.ViewModels;

public partial class RequestTabViewModel : ViewModelBase
{
    private readonly GreatRequestExecutor _executor;

    [ObservableProperty]
    private int? _requestId;

    [ObservableProperty]
    private int? _collectionId;

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
    private bool _isSaved = false;

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

    // Use TextDocument for script editors
    [ObservableProperty]
    private TextDocument _scriptDocument = new();

    [ObservableProperty]
    private TextDocument _postScriptDocument = new();

    [ObservableProperty]
    private IHighlightingDefinition? _scriptSyntaxHighlighting;

    [ObservableProperty]
    private string _scriptLanguage = "C#";

    [ObservableProperty]
    private bool _isVariablesGuideOpen;

    [ObservableProperty]
    private bool _isSnippetsPanelOpen;

    public List<string> StandardVariables => StandardVariablesProvider.SupportedVariables.ToList();

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
    
    public void OnQueryParamUpdated()
    {
        // Get base URL without query params
        var urlParts = Url.Split('?');
        var baseUrl = urlParts[0];
        
        // Get non-empty query params
        var activeParams = QueryParams
            .Where(q => !string.IsNullOrWhiteSpace(q.Key))
            .Select(q => $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(q.Value)}")
            .ToList();
        
        // Rebuild URL with query params
        if (activeParams.Any())
        {
            Url = $"{baseUrl}?{string.Join("&", activeParams)}";
        }
        else if (Url.Contains("?"))
        {
            Url = baseUrl;
        }
    }

    [RelayCommand]
    private void SelectOptionTab(string tab)
    {
        SelectedOptionTab = tab;
        OnPropertyChanged(nameof(CurrentSnippets));
    }

    [RelayCommand]
    private void SelectResponseTab(string tab)
    {
        SelectedResponseTab = tab;
    }

    [RelayCommand]
    private void AddHeader()
    {
        var header = new HeaderViewModel
        {
            Key = "",
            Value = ""
        };
        header.KeyChanged += (s, e) => OnHeaderKeyChanged(header);
        Headers.Add(header);
    }

    [RelayCommand]
    private void RemoveHeader(HeaderViewModel header)
    {
        Headers.Remove(header);
    }

    // Auto-add new header row when last row is filled
    public void OnHeaderKeyChanged(HeaderViewModel header)
    {
        if (Headers.Count > 0 && Headers.Last() == header && !string.IsNullOrWhiteSpace(header.Key))
        {
            AddHeader();
        }
    }

    [RelayCommand]
    private void AddQueryParam()
    {
        var param = new QueryParamViewModel
        {
            Key = "",
            Value = ""
        };
        param.KeyChanged += (s, e) => OnQueryParamKeyChanged(param);
        param.ValueChanged += (s, e) => OnQueryParamUpdated();
        QueryParams.Add(param);
    }

    [RelayCommand]
    private void RemoveQueryParam(QueryParamViewModel param)
    {
        QueryParams.Remove(param);
        OnQueryParamUpdated();
    }

    // Auto-add new query param row when last row is filled
    public void OnQueryParamKeyChanged(QueryParamViewModel param)
    {
        if (QueryParams.Count > 0 && QueryParams.Last() == param && !string.IsNullOrWhiteSpace(param.Key))
        {
            AddQueryParam();
        }
        OnQueryParamUpdated();
    }

    [RelayCommand]
    private void ToggleVariablesGuide()
    {
        IsVariablesGuideOpen = !IsVariablesGuideOpen;
        if (IsVariablesGuideOpen)
            IsSnippetsPanelOpen = false;
    }

    [RelayCommand]
    private void ToggleSnippetsPanel()
    {
        IsSnippetsPanelOpen = !IsSnippetsPanelOpen;
        if (IsSnippetsPanelOpen)
            IsVariablesGuideOpen = false;
    }

    [RelayCommand]
    private void InsertSnippet(CodeSnippet snippet)
    {
        if (SelectedOptionTab == "PreScript")
        {
            ScriptDocument.Text += "\n" + snippet.Code;
        }
        else if (SelectedOptionTab == "PostScript")
        {
            PostScriptDocument.Text += "\n" + snippet.Code;
        }
    }

    public List<CodeSnippet> CurrentSnippets
    {
        get
        {
            if (SelectedOptionTab == "PreScript")
            {
                return ScriptLanguage == "C#" ? CodeSnippets.CSharpPreRequestSnippets : CodeSnippets.PythonPreRequestSnippets;
            }
            else if (SelectedOptionTab == "PostScript")
            {
                return ScriptLanguage == "C#" ? CodeSnippets.CSharpPostRequestSnippets : CodeSnippets.PythonPostRequestSnippets;
            }
            return new List<CodeSnippet>();
        }
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
        
        // Apply authentication headers before sending
        ApplyAuthHeaders();
        
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
        var scriptToUse = ScriptDocument.Text;
        var postScriptToUse = PostScriptDocument.Text;
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
                    ResponseSyntaxHighlighting = SyntaxHighlightingHelper.GetJsonHighlighting();
                }
                else if (response.BodyType?.Contains("xml") == true)
                {
                    ResponseSyntaxHighlighting = SyntaxHighlightingHelper.GetXmlHighlighting();
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

    public string GetContentType()
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
            "JSON" => SyntaxHighlightingHelper.GetJsonHighlighting(),
            "XML" => SyntaxHighlightingHelper.GetXmlHighlighting(),
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
            // Ignore standard variables for JSON validation by temporarily replacing them
            var textToValidate = System.Text.RegularExpressions.Regex.Replace(
                BodyDocument.Text, 
                @"\{\$[^{}]+\}|\{![^{}]+\}", 
                "\"TEMPORARY_VARIABLE_PLACEHOLDER\"");

            JsonDocument.Parse(textToValidate);
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
        // Start with just empty rows
        var emptyHeader = new HeaderViewModel
        {
            Key = "",
            Value = ""
        };
        emptyHeader.KeyChanged += (s, e) => OnHeaderKeyChanged(emptyHeader);
        Headers.Add(emptyHeader);

        var emptyParam = new QueryParamViewModel
        {
            Key = "",
            Value = ""
        };
        emptyParam.KeyChanged += (s, e) => OnQueryParamKeyChanged(emptyParam);
        emptyParam.ValueChanged += (s, e) => OnQueryParamUpdated();
        QueryParams.Add(emptyParam);

        ScriptDocument.Text = "";
        PostScriptDocument.Text = "";
        Url = "";
        
        // Initialize syntax highlighting for JSON and Scripts
        UpdateBodySyntaxHighlighting();
        UpdateScriptSyntaxHighlighting();
    }

    private void ApplyAuthHeaders()
    {
        // Remove existing Authorization header if present
        var existingAuthHeader = Headers.FirstOrDefault(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase));
        if (existingAuthHeader != null)
        {
            Headers.Remove(existingAuthHeader);
        }

        // Add new Authorization header based on auth type
        string? authValue = null;
        
        switch (AuthType)
        {
            case "Bearer Token":
                if (!string.IsNullOrWhiteSpace(AuthToken))
                {
                    authValue = $"Bearer {AuthToken}";
                }
                break;
            
            case "Basic Auth":
                if (!string.IsNullOrWhiteSpace(AuthUsername))
                {
                    var credentials = $"{AuthUsername}:{AuthPassword}";
                    var bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
                    var base64 = Convert.ToBase64String(bytes);
                    authValue = $"Basic {base64}";
                }
                break;
            
            case "API Key":
                if (!string.IsNullOrWhiteSpace(AuthToken))
                {
                    authValue = AuthToken;
                }
                break;
        }

        if (authValue != null)
        {
            var authHeader = new HeaderViewModel
            {
                Key = "Authorization",
                Value = authValue
            };
            authHeader.KeyChanged += (s, e) => OnHeaderKeyChanged(authHeader);
            Headers.Insert(0, authHeader); // Insert at top
        }
    }

    partial void OnScriptLanguageChanged(string value)
    {
        UpdateScriptSyntaxHighlighting();
        OnPropertyChanged(nameof(CurrentSnippets));
    }

    private void UpdateScriptSyntaxHighlighting()
    {
        ScriptSyntaxHighlighting = ScriptLanguage switch
        {
            "C#" => SyntaxHighlightingHelper.GetCSharpHighlighting(),
            "Python" => SyntaxHighlightingHelper.GetPythonHighlighting(),
            _ => null
        };
    }

    /// <summary>
    /// Load request data from database model into the tab
    /// </summary>
    public void LoadFromRequest(Request request)
    {
        RequestId = request.Id;
        CollectionId = request.CollectionId;
        Name = request.Name;
        Method = request.Method;
        Url = request.Url;
        BodyType = string.IsNullOrEmpty(request.BodyType) ? "None" : 
                   request.BodyType.Contains("json") ? "JSON" :
                   request.BodyType.Contains("xml") ? "XML" :
                   request.BodyType.Contains("form") ? "Form Data" : "Raw";
        
        if (!string.IsNullOrEmpty(request.Body))
        {
            BodyDocument.Text = request.Body;
        }

        // Load headers
        Headers.Clear();
        foreach (var header in request.Headers)
        {
            var h = new HeaderViewModel
            {
                Key = header.Key,
                Value = header.Value
            };
            h.KeyChanged += (s, e) => OnHeaderKeyChanged(h);
            Headers.Add(h);
        }
        var emptyH = new HeaderViewModel();
        emptyH.KeyChanged += (s, e) => OnHeaderKeyChanged(emptyH);
        Headers.Add(emptyH); // Add empty row

        // Load query params
        QueryParams.Clear();
        foreach (var param in request.Query)
        {
            var p = new QueryParamViewModel
            {
                Key = param.Key,
                Value = param.Value
            };
            p.KeyChanged += (s, e) => OnQueryParamKeyChanged(p);
            p.ValueChanged += (s, e) => OnQueryParamUpdated();
            QueryParams.Add(p);
        }
        var emptyP = new QueryParamViewModel();
        emptyP.KeyChanged += (s, e) => OnQueryParamKeyChanged(emptyP);
        emptyP.ValueChanged += (s, e) => OnQueryParamUpdated();
        QueryParams.Add(emptyP); // Add empty row

        // Load scripts
        if (!string.IsNullOrEmpty(request.PreRequestScript))
        {
            ScriptDocument.Text = request.PreRequestScript;
        }
        
        if (!string.IsNullOrEmpty(request.PostResponseScript))
        {
            PostScriptDocument.Text = request.PostResponseScript;
        }

        if (request.ScriptLanguage.HasValue)
        {
            ScriptLanguage = request.ScriptLanguage.Value == Data.Models.Enums.Languages.Csharp ? "C#" : "Python";
        }

        IsSaved = true;
    }

    /// <summary>
    /// Load request data from history entry into the tab
    /// </summary>
    public void LoadFromHistory(HistoryEntry historyEntry)
    {
        LoadFromRequest(historyEntry.Request);
        
        // Clear CollectionId and RequestId so it can be saved as a new request
        CollectionId = null;
        RequestId = null;
        IsSaved = false;
    }
}