using System.Reflection.Metadata;
using Hazelcast;

namespace Messages.Controllers;

public class HazelcastWorker<TValue>
{
    private readonly IHazelcastClient client;
    private readonly string queue;
    public HazelcastWorker(IHazelcastClient _client, string _queue)
    {
        client = _client;
        queue = _queue;
    }

    public async Task<TValue> GetRecordAsync()
    {
        var local_queue = await client.GetQueueAsync<TValue>(queue)
            .ConfigureAwait(false);
            //await client.GetMapAsync<TKey, TValue>(map)
            //.ConfigureAwait(false);
            var rec = await local_queue.GetElementAsync().ConfigureAwait(false);
        //local_queue.GetAsync(key).ConfigureAwait(false);
        return rec;
    }
    public async Task<Queue<TValue>> GetRecordsAsync()
    {
        var local_queue = await client.GetQueueAsync<TValue>(queue)
            .ConfigureAwait(false);
        // GetEntriesAsync().ConfigureAwait(false)
        // GetAllAsync(new System.Collections.Generic.List<TKey>()).ConfigureAwait(false);
        var rec = await local_queue.GetAllAsync().ConfigureAwait(false);
        
        // convert rec to a dictionary of TKey, TValue
        var little_queue = new Queue<TValue>();
        foreach (var item in rec)
        {
            // add to little_queue 
            little_queue.Enqueue(item);
        }
        
        return little_queue;
    }
    /*public async Task SetRecordAsync(TValue value)
    {
        var local_queue = await client.GetQueueAsync<TValue>(queue)
            .ConfigureAwait(false);
        await local_queue.PutAsync(value).ConfigureAwait(false);
    }*/

    public async Task PutRecordAsync(TValue value)
    {
        var local_queue = await client.GetQueueAsync<TValue>(queue)
            .ConfigureAwait(false);
        await local_queue.PutAsync(value).ConfigureAwait(false);
    }

    /*public async Task DeleteRecordAsync()
    {
        var local_queue = await client.GetQueueAsync<TValue>(queue)
            .ConfigureAwait(false);
        await local_queue.DeleteAsync(key);
    }*/
}