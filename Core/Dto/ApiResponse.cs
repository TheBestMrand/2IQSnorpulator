namespace Data.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public bool IsSuccessStatusCode => StatusCode is >= 200 and < 300;
    
    public string? Body { get; set; }
    public string? BodyType { get; set; }
    
    public Dictionary<string, string> Headers { get; set; } = new();
    public TimeSpan ResponseTime { get; set; }
    
    public int Size { get; set; }
    
}