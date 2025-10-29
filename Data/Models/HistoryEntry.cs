namespace Data.Models;

public class HistoryEntry : Request
{
    public DateTime ExecutedAt { get; set; }
    
    public int StatusCode { get; set; }
    public string ResponseBody { get; set; }
    public long ResponseTimeMs { get; set; }
    
    public int? RequestId { get; set; }
}