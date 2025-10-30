using Data.Consts;
using LiteDB;

namespace Data.Models;

public class Collection : CollectionLite
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    [BsonRef(DbConsts.TableNameRequests)]
    public List<Request> Requests { get; set; } = new();
}