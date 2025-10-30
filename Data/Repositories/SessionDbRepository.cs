using System.Linq.Expressions;
using Data.Database;
using Data.Models;
using LiteDB;

namespace Data.Repositories;

public class SessionDbRepository
{
    private readonly DbService _dbService;
    private readonly ILiteCollection<Session> _collection;

    public SessionDbRepository(DbService dbService)
    {
        _dbService = dbService;
        _collection = _dbService.Session;
    }

    public int Insert(Session entity)
    {
        return _collection.Insert(entity).AsInt32;
    }

    public Session? GetById(int id)
    {
        return _collection.FindById(id);
    }

    public bool Update(Session entity)
    {
        return _collection.Update(entity);
    }

    public IEnumerable<Session> Find(Expression<Func<Session, bool>> predicate)
    {
        return _collection.Find(predicate);
    }
}