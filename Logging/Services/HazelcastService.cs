using Hazelcast;
using Logging.Controllers;

namespace Logging.Services;

public class HazelcastService<TKey, TValue>:IHazelcastService<TKey,TValue>
{
    private readonly IHazelcastClient client;
    private readonly string map;

    public HazelcastService(HazelcastOptions options, string new_map)
    {
        client = HazelcastClientFactory.GetNewStartingClient(options).Client;
        map = new_map;
    }

    public async Task<TValue> GetRecordAsync(TKey key)
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
        var rec = await worker.GetRecordAsync(key).ConfigureAwait(false);
        return rec;
    }
    public async Task<Dictionary<TKey, TValue>> GetRecordsAsync()
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
        var rec = await worker.GetRecordsAsync().ConfigureAwait(false);
        return rec;
    }
    
    public async Task SetRecordAsync(TKey key, TValue value)
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
       await worker.SetRecordAsync(key, value).ConfigureAwait(false);
    }
    
    public async Task PutRecordAsync(TKey key, TValue value)
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
        await worker.PutRecordAsync(key, value).ConfigureAwait(false);
    }
    
    public async Task DeleteRecordAsync(TKey key)
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
        await worker.DeleteRecordAsync(key).ConfigureAwait(false);
    }
}