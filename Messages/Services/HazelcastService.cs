using Hazelcast;
using Messages.Controllers;

namespace Messages.Services;

public class HazelcastService<TValue>:IHazelcastService<TValue>
{
    private readonly IHazelcastClient client;
    private readonly string queue;

    public HazelcastService(HazelcastOptions options, string new_queue)
    {
        client = HazelcastClientFactory.GetNewStartingClient(options).Client;
        queue = new_queue;
    }

    public async Task<TValue> GetRecordAsync()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<HazelcastWorker<TValue>>();
        
        var worker = new HazelcastWorker<TValue>(client, queue);
        var rec = await worker.GetRecordAsync().ConfigureAwait(false);
        
        logger.LogInformation("HazelcastService->getRecordAsync: " + rec);
        
        return rec;
    }
    public async Task<Queue<TValue>> GetRecordsAsync()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<HazelcastWorker<TValue>>();
        
        var worker = new HazelcastWorker<TValue>(client, queue);
        var rec = await worker.GetRecordsAsync().ConfigureAwait(false);
        
        // show through the logger rec
        logger.LogInformation("HazelcastService->getRecordsAsync: " + rec);
        
        return rec;
    }
    
    /*public async Task SetRecordAsync(TValue value)
    {
        var worker = new HazelcastWorker<TValue>(client, map);
       await worker.SetRecordAsync(value).ConfigureAwait(false);
    }*/
    
    public async Task PutRecordAsync(TValue value)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<HazelcastWorker<TValue>>();
        
        
        logger.LogInformation("HazelcastService->putRecordAsync: " + value);
        
        var worker = new HazelcastWorker<TValue>(client, queue);
        await worker.PutRecordAsync(value).ConfigureAwait(false);
    }
    
    /*public async Task DeleteRecordAsync(TKey key)
    {
        var worker = new HazelcastWorker<TKey, TValue>(client, map);
        await worker.DeleteRecordAsync(key).ConfigureAwait(false);
    }*/
}