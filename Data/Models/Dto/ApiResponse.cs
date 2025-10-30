namespace Data.Models.Dto;

public class ApiResponse
{
    public int StatusCode { get; set; }
    
    public string? Body { get; set; }
    public string? BodyType { get; set; }
    
    public Dictionary<string, string> Headers { get; set; } = new();
    public TimeSpan ResponseTime { get; set; }
    
    public int Size { get; set; }
    
}