namespace Logging.Services;

public interface IHazelcastService<TKey,TValue>
{
    Task<TValue> GetRecordAsync(TKey key);
    Task SetRecordAsync(TKey key, TValue value);
    Task PutRecordAsync(TKey key, TValue value);
    Task DeleteRecordAsync(TKey key);
    Task<Dictionary<TKey, TValue>> GetRecordsAsync();
}