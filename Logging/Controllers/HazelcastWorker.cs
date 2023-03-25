using System.Reflection.Metadata;
using Hazelcast;

namespace Logging.Controllers;

public class HazelcastWorker<TKey, TValue>
{
    private readonly IHazelcastClient client;
    private readonly string map;
    public HazelcastWorker(IHazelcastClient _client, string _map)
    {
        client = _client;
        map = _map;
    }

    public async Task<TValue> GetRecordAsync(TKey key)
    {
        var local_map = await client.GetMapAsync<TKey, TValue>(map)
            .ConfigureAwait(false);
        var rec = await local_map.GetAsync(key).ConfigureAwait(false);
        return rec;
    }
    public async Task<Dictionary<TKey, TValue>> GetRecordsAsync()
    {
        var local_map = await client.GetMapAsync<TKey, TValue>(map)
            .ConfigureAwait(false);
        // GetEntriesAsync().ConfigureAwait(false)
        // GetAllAsync(new System.Collections.Generic.List<TKey>()).ConfigureAwait(false);
        var rec = await local_map.GetEntriesAsync().ConfigureAwait(false);
        
        // convert rec to a dictionary of TKey, TValue
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in rec)
        {
            dict.Add(item.Key, item.Value);
        }
        
        return dict;
    }
    public async Task SetRecordAsync(TKey key, TValue value)
    {
        var local_map = await client.GetMapAsync<TKey, TValue>(map)
            .ConfigureAwait(false);
        await local_map.SetAsync(key, value).ConfigureAwait(false);
    }

    public async Task PutRecordAsync(TKey key, TValue value)
    {
        var local_map = await client.GetMapAsync<TKey, TValue>(map)
            .ConfigureAwait(false);
        await local_map.PutAsync(key, value).ConfigureAwait(false);
    }

    public async Task DeleteRecordAsync(TKey key)
    {
        var local_map = await client.GetMapAsync<TKey, TValue>(map)
            .ConfigureAwait(false);
        await local_map.DeleteAsync(key);
    }
}