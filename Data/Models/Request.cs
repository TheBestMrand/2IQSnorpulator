namespace Data.Models;

public class Request
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int CollectionId { get; set; }
    
    public string Method { get; set; }
    public string Url { get; set; }
    public string Body { get; set; }
    
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    
    public string PreRequestScript { get; set; }
    public string PostResponseScript { get; set; }
}