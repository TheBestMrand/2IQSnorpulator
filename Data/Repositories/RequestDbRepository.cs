using System.Linq.Expressions;
using Data.Database;
using Data.Models;
using LiteDB;

namespace Data.Repositories;

public class RequestDbRepository
{
    private readonly DbService _dbService;
    private readonly ILiteCollection<Request> _collection;

    public RequestDbRepository(DbService dbService)
    {
        _dbService = dbService;
        _collection = _dbService.Requests;
    }

    public int Insert(Request entity)
    {
        return _collection.Insert(entity).AsInt32;
    }

    public Request? GetById(int id)
    {
        return _collection.FindById(id);
    }

    public IEnumerable<Request> GetAll()
    {
        return _collection.FindAll();
    }

    public bool Update(Request entity)
    {
        return _collection.Update(entity);
    }

    public bool Delete(int id)
    {
        return _collection.Delete(id);
    }

    public IEnumerable<Request> Find(Expression<Func<Request, bool>> predicate)
    {
        return _collection.Find(predicate);
    }
}