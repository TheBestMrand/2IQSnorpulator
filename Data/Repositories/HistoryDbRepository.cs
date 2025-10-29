using System.Linq.Expressions;
using Data.Database;
using Data.Models;
using LiteDB;

namespace Data.Repositories;

public class HistoryDbRepository
{
    private readonly DbService _dbService;
    private readonly ILiteCollection<HistoryEntry> _collection;

    public HistoryDbRepository(DbService dbService)
    {
        _dbService = dbService;
        _collection = _dbService.History;
    }

    public int Insert(HistoryEntry entity)
    {
        return _collection.Insert(entity).AsInt32;
    }

    public HistoryEntry? GetById(int id)
    {
        return _collection.FindById(id);
    }

    public IEnumerable<HistoryEntry> GetAll()
    {
        return _collection.FindAll();
    }

    public bool Delete(int id)
    {
        return _collection.Delete(id);
    }

    public IEnumerable<HistoryEntry> Find(Expression<Func<HistoryEntry, bool>> predicate)
    {
        return _collection.Find(predicate);
    }
}