using Data.Consts;
using Data.Models;
using LiteDB;
using Environment = Data.Models.Environment;

namespace Data.Database;

public class DbService : IDisposable
{
    private readonly Lazy<LiteDatabase> _db;
    
    // Lazy collection properties for better performance
    private ILiteCollection<Collection>? _collections;
    private ILiteCollection<Request>? _requests;
    private ILiteCollection<Environment>? _environments;
    private ILiteCollection<HistoryEntry>? _history;
    private ILiteCollection<Session>? _session;
    
    public ILiteCollection<Collection> Collections => _collections ??= GetCollection<Collection>(DbConsts.TableNameCollections);
    public ILiteCollection<Request> Requests => _requests ??= GetCollection<Request>(DbConsts.TableNameRequests);
    public ILiteCollection<Environment> Environments => _environments ??= GetCollection<Environment>(DbConsts.TableNameEnvironments);
    public ILiteCollection<HistoryEntry> History => _history ??= GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries);
    public ILiteCollection<Session> Session => _session ??= GetCollection<Session>(DbConsts.TableSessionEntries);
    
    private string GetDbPath()
    {
        var dbFolder = AppContext.BaseDirectory; 
        var dbPath = Path.Combine(dbFolder, DbConsts.DbFileName);
        
        return dbPath;
    }

    public DbService()
    {
        // Lazy initialization for faster startup
        _db = new Lazy<LiteDatabase>(() =>
        {
            var connection = new LiteDatabase(GetDbPath());
            SetupIndexes(connection);
            return connection;
        }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    
    private ILiteCollection<T> GetCollection<T>(string name)
    {
        return _db.Value.GetCollection<T>(name);
    }
    
    private void SetupIndexes(LiteDatabase db)
    {
        // Setup indexes on collections - pass db directly to avoid circular dependency
        db.GetCollection<Request>(DbConsts.TableNameRequests).EnsureIndex(x => x.CollectionId);
        db.GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries).EnsureIndex(x => x.ExecutedAt);
        db.GetCollection<HistoryEntry>(DbConsts.TableNameHistoryEntries).EnsureIndex(x => x.Name);
        db.GetCollection<Collection>(DbConsts.TableNameCollections).EnsureIndex(x => x.Name, true);
        db.GetCollection<Environment>(DbConsts.TableNameEnvironments).EnsureIndex(x => x.Name, true);
        db.GetCollection<Session>(DbConsts.TableSessionEntries).EnsureIndex(x => x.Name, true);
    }

    public void Dispose()
    {
        if (_db.IsValueCreated)
        {
            _db.Value.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}