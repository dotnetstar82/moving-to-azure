namespace MovingToAzure.Data;

public interface ICacheRepository
{
    T? GetOrLoad<T>(string key, Func<T> Load) where T : class;
    void Save<T>(string key, T obj) where T : class;
    void Clear(string key);
    T? Get<T>(string key) where T : class;
}

public class CacheRepository : ICacheRepository
{
    private static readonly Dictionary<string, object> cache = new Dictionary<string, object>();
    private static readonly object thelock = new object();

    public CacheRepository()
    {
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
        // FRAGILE: not thread safe:
        if (cache.ContainsKey(key))
        {
            cache[key] = obj;
        } else
        {
            cache.Add(key, obj);
        }
    }

    public void Clear(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        // FRAGILE: not thread safe:
        if (cache.ContainsKey(key))
        {
            cache.Remove(key);
        }
    }

    public T? Get<T>(string key) where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        cache.TryGetValue(key, out var obj);
        return (T?)obj; // FRAGILE: ASSUME: it's the right type
    }

}
