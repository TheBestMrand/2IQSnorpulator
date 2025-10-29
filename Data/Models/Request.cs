using Data.Models.Enums;

namespace Data.Models;

public class Request
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int CollectionId { get; set; }
    
    public string Method { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    public string? Body { get; set; }
    public string? BodyType { get; set; }
    
    public Dictionary<string, string> Query { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    
    public string? PreRequestScript { get; set; }
    public string? PostResponseScript { get; set; }
    public Languages? ScriptLanguage { get; set; }
    
    //TODO: Auth
}