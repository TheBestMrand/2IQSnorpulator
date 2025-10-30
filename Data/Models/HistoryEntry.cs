using Data.Models.Dto;

namespace Data.Models;

public class HistoryEntry : CollectionLite
{
    public Request Request { get; set; } = new();
    public ApiResponse Response { get; set; } = new();
    
    public DateTime ExecutedAt { get; } = DateTime.UtcNow;
}