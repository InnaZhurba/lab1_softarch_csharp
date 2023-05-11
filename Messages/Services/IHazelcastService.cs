namespace Messages.Services;

public interface IHazelcastService<TValue>
{
    Task<TValue> GetRecordAsync();
    //Task SetRecordAsync(TValue value);
    Task PutRecordAsync(TValue value);
    //Task DeleteRecordAsync();
    Task<Queue<TValue>> GetRecordsAsync();
}