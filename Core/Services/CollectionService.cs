using Data.Models;
using Data.Repositories;

namespace Core.Services;

/// <summary>
/// High-performance service for managing collections and requests
/// </summary>
public class CollectionService
{
    private readonly CollectionDbRepository _collectionRepo;
    private readonly RequestDbRepository _requestRepo;
    private readonly HistoryDbRepository _historyRepo;

    public CollectionService(
        CollectionDbRepository collectionRepo,
        RequestDbRepository requestRepo,
        HistoryDbRepository historyRepo)
    {
        _collectionRepo = collectionRepo;
        _requestRepo = requestRepo;
        _historyRepo = historyRepo;
    }

    // ============ COLLECTIONS ============
    
    public Collection CreateCollection(string name)
    {
        var collection = new Collection { Name = name };
        collection.Id = _collectionRepo.Insert(collection);
        return collection;
    }

    public bool RenameCollection(int collectionId, string newName)
    {
        var collection = _collectionRepo.GetById(collectionId);
        if (collection == null) return false;
        
        collection.Name = newName;
        return _collectionRepo.Update(collection);
    }

    public bool DeleteCollection(int collectionId)
    {
        // Delete all requests in the collection
        var collection = _collectionRepo.GetById(collectionId);
        if (collection == null) return false;

        foreach (var request in collection.Requests)
        {
            _requestRepo.Delete(request.Id);
        }

        return _collectionRepo.Delete(collectionId);
    }

    public IEnumerable<Collection> GetAllCollections()
    {
        return _collectionRepo.GetAll();
    }

    public Collection? GetCollectionById(int id)
    {
        return _collectionRepo.GetById(id);
    }

    // ============ REQUESTS ============
    
    public Request CreateRequest(string name, int collectionId)
    {
        var request = new Request 
        { 
            Name = name,
            CollectionId = collectionId,
            Method = "GET",
            Url = ""
        };
        
        request.Id = _requestRepo.Insert(request);
        return request;
    }

    public Request SaveRequestFromTab(
        string name,
        string method,
        string url,
        string? body,
        string? bodyType,
        Dictionary<string, string> headers,
        Dictionary<string, string> query,
        string? preRequestScript,
        string? postResponseScript,
        Data.Models.Enums.Languages? scriptLanguage,
        int collectionId,
        int? existingRequestId = null)
    {
        var request = new Request
        {
            Name = name,
            Method = method,
            Url = url,
            Body = body,
            BodyType = bodyType,
            Headers = headers,
            Query = query,
            PreRequestScript = preRequestScript,
            PostResponseScript = postResponseScript,
            ScriptLanguage = scriptLanguage,
            CollectionId = collectionId
        };

        if (existingRequestId.HasValue)
        {
            request.Id = existingRequestId.Value;
            _requestRepo.Update(request);
        }
        else
        {
            request.Id = _requestRepo.Insert(request);
        }

        return request;
    }

    public bool UpdateRequest(Request request)
    {
        return _requestRepo.Update(request);
    }

    public bool DeleteRequest(int requestId)
    {
        return _requestRepo.Delete(requestId);
    }

    public Request? GetRequestById(int id)
    {
        return _requestRepo.GetById(id);
    }

    public IEnumerable<Request> GetRequestsByCollectionId(int collectionId)
    {
        return _requestRepo.Find(r => r.CollectionId == collectionId);
    }

    // ============ HISTORY ============
    
    public IEnumerable<HistoryEntry> GetRecentHistory(int limit = 50)
    {
        // Get history sorted by ExecutedAt descending (most recent first)
        return _historyRepo.GetAll()
            .OrderByDescending(h => h.ExecutedAt)
            .Take(limit);
    }

    public bool DeleteHistoryEntry(int id)
    {
        return _historyRepo.Delete(id);
    }

    public bool ClearHistory()
    {
        var allHistory = _historyRepo.GetAll().ToList();
        foreach (var entry in allHistory)
        {
            _historyRepo.Delete(entry.Id);
        }
        return true;
    }
}

