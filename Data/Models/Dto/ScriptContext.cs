namespace Data.Models.Dto;

public class ScriptContext
{
    public Dictionary<string, string> Environment { get; set; } = new();
    public Dictionary<string, string> Variables { get; set; } = new(); // Script-accessible variables
    public Dictionary<string, string> ResolvedVariables { get; set; } = new(); // Cache for generated values
    public ApiResponse? Response { get; set; }
    public Request? Request { get; set; }
    
    //Request
    public string Url
    {
        get => Request?.Url ?? string.Empty;
        set { if (Request != null) Request.Url = value; }
    }

    public string Method
    {
        get => Request?.Method ?? string.Empty;
        set { if (Request != null) Request.Method = value; }
    }

    public string? Body
    {
        get => Request?.Body;
        set { if (Request != null) Request.Body = value; }
    }
    
    public string? BodyType
    {
        get => Request?.BodyType;
        set { if (Request != null) Request.BodyType = value; }
    }

    public Dictionary<string, string> Headers
    {
        get => Request?.Headers ?? new Dictionary<string, string>();
        set { if (Request != null) Request.Headers = value; }
    }
    
    public Dictionary<string, string> Query
    {
        get => Request?.Query ?? new Dictionary<string, string>();
        set { if (Request != null) Request.Query = value; }
    }
    
    //Response
    public int StatusCode => Response?.StatusCode ?? 0;
    
    public string? ResponseBody => Response?.Body;

    public string? ResponseBodyType => Response?.BodyType;

    public Dictionary<string, string> ResponseHeaders => Response?.Headers ?? new Dictionary<string, string>();

    public TimeSpan ResponseTime => Response?.ResponseTime ?? TimeSpan.Zero;
    
    public int ResponseSize => Response?.Size ?? 0;
}