using System.Linq.Expressions;
using Data.Database;
using LiteDB;
using Environment = Data.Models.Environment;

namespace Data.Repositories;

public class EnvironmentDbRepository
{
    private readonly DbService _dbService;
    private readonly ILiteCollection<Environment> _collection;

    public EnvironmentDbRepository(DbService dbService)
    {
        _dbService = dbService;
        _collection = _dbService.Environments;
    }

    public int Insert(Environment entity)
    {
        return _collection.Insert(entity).AsInt32;
    }

    public Environment? GetById(int id)
    {
        return _collection.FindById(id);
    }

    public IEnumerable<Environment> GetAll()
    {
        return _collection.FindAll();
    }

    public bool Update(Environment entity)
    {
        return _collection.Update(entity);
    }

    public bool Delete(int id)
    {
        return _collection.Delete(id);
    }

    public IEnumerable<Environment> Find(Expression<Func<Environment, bool>> predicate)
    {
        return _collection.Find(predicate);
    }
}