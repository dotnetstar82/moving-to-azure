using MovingToAzure.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace MovingToAzure.Data;

public interface ICacheRepository
{
    T? GetOrLoad<T>(string key, Func<T> Load) where T : class;
    void Save<T>(string key, T obj) where T : class;
    void Clear(string key);
    T? Get<T>(string key) where T : class;
}

// https://docs.redis.com/latest/rs/references/client_references/client_csharp/
public class CacheRepository : ICacheRepository
{
    private readonly IConnectionMultiplexer redis;
    private readonly TimeSpan cacheTimeout;
    private readonly object thelock = new object();

    public CacheRepository(IConnectionMultiplexer redis, AppSettings settings)
    {
        this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }
        cacheTimeout = TimeSpan.FromSeconds(settings.CacheSeconds);
    }

    public T? GetOrLoad<T>(string key, Func<T> Load) where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        T? obj = Get<T>(key);
        if (obj == null)
        {
            lock(thelock)
            {
                obj = Get<T>(key);
                if (obj == null) {
                    obj = Load();
                    Save<T>(key, obj);
                }
            }
        }
        return obj;
    }

    public void Save<T>(string key, T obj) where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (obj == null)
        {
            Clear(key);
            return;
        }
        string val = JsonSerializer.Serialize(obj);
        var db = redis.GetDatabase();
        db.StringSet(key, val, cacheTimeout); // upsert
    }

    public void Clear(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        var db = redis.GetDatabase();
        db.KeyDeleteAsync(key);
    }

    public T? Get<T>(string key) where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        var db = redis.GetDatabase();
        string? val = db.StringGet(key);
        if (string.IsNullOrEmpty(val))
        {
            return null;
        }
        T? obj = JsonSerializer.Deserialize<T>(val); // FRAGILE: ASSUME: text is in correct format
        return obj;
    }

}
