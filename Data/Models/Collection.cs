using Data.Consts;
using LiteDB;

namespace Data.Models;

public class Collection
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    [BsonRef(DbConsts.TableNameRequests)]
    public List<Request> Requests { get; set; } = new();
}