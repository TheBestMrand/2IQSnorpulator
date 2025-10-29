using Data.Consts;
using Data.Models;
using LiteDB;
using Environment = Data.Models.Environment;

namespace Data.Database;

public class DbService : IDisposable
{
    private readonly LiteDatabase _db;
    
    public ILiteCollection<Collection> Collections => _db.GetCollection<Collection>(DbConsts.TableNameCollections);
    public ILiteCollection<Request> Requests => _db.GetCollection<Request>(DbConsts.TableNameRequests);
    public ILiteCollection<Environment> Environments => _db.GetCollection<Environment>(DbConsts.TableNameEnvironments);
    public ILiteCollection<HistoryEntry> History => _db.GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries);
    
    private string GetDbPath()
    {
        var dbFolder = AppContext.BaseDirectory; 
        var dbPath = Path.Combine(dbFolder, DbConsts.DbFileName);
        
        return dbPath;
    }

    public DbService()
    {
        _db = new LiteDatabase(GetDbPath());
        
        SetupIndexes();
    }
    
    private ILiteCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);
    
    private void SetupIndexes()
    {
        GetCollection<Request>(DbConsts.TableNameRequests).EnsureIndex(x => x.CollectionId);
        GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries).EnsureIndex(x => x.ExecutedAt);
        GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries).EnsureIndex(x => x.Name, true);
        GetCollection<Collection>(DbConsts.TableNameCollections).EnsureIndex(x => x.Name, true);
        GetCollection<Environment>(DbConsts.TableNameEnvironments).EnsureIndex(x => x.Name, true);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}