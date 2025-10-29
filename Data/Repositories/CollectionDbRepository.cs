using System.Linq.Expressions;
using Data.Database;
using Data.Models;
using LiteDB;

namespace Data.Repositories;

public class CollectionDbRepository
{
    private readonly DbService _dbService;
    private readonly ILiteCollection<Collection> _collection;

    public CollectionDbRepository(DbService dbService)
    {
        _dbService = dbService;
        _collection = _dbService.Collections;
    }

    public int Insert(Collection entity)
    {
        return _collection.Insert(entity).AsInt32;
    }
    
    public bool LinkRequest(Collection entity, Request request)
    {
        var collection = _collection.FindById(entity.Id);

        if (collection == null)
        {
            return false;
        }
        
        collection.Requests.Add(request);
        return _collection.Update(entity);
    }

    public Collection? GetById(int id)
    {
        return _collection.FindById(id);
    }

    public IEnumerable<Collection> GetAll()
    {
        return _collection.FindAll();
    }

    public bool Update(Collection entity)
    {
        return _collection.Update(entity);
    }

    public bool Delete(int id)
    {
        return _collection.Delete(id);
    }
    
    public bool UnlinkRequest(int collectionId, int requestId)
    {
        var collection = _collection.FindById(collectionId);

        if (collection == null)
        {
            return false;
        }
        
        var requestToRemove = collection.Requests.FirstOrDefault(r => r.Id == requestId);
        
        if (requestToRemove == null)
        {
            return false;
        }
        
        collection.Requests.Remove(requestToRemove);
        
        return _collection.Update(collection);
    }

    public IEnumerable<Collection> Find(Expression<Func<Collection, bool>> predicate)
    {
        return _collection.Find(predicate);
    }
}