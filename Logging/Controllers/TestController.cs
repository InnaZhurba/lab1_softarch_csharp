using System.Collections.Specialized;
using Logging.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Logging.Services;

namespace Logging.Controllers;

public class TestController
{
    private readonly IHazelcastService<int, string> service;
    private readonly ILogger _logger;

    public TestController(ILoggerFactory loggerFactory, Services.IHazelcastService<int, string> _service)
    {
        service = _service;
        _logger = loggerFactory.CreateLogger<LoggingController>();
    }

    [HttpGet]
    [Route("test/get-attempt/{id}")]
    public async Task<JsonResult> GetAttempt(int id)
    {
        var rec = await service.GetRecordAsync(id).ConfigureAwait(false);
        
        return new JsonResult(rec);
    }
    
    [HttpGet]
    [Route("test/get-all-attempts")]
    public async Task<string> GetAttempts()
    //public async Task<IReadOnlyDictionary<string,int>> GetAttempts()
    {
        var rec = await service.GetRecordsAsync().ConfigureAwait(false);
        List<string> list_messages = new List<string>();
        
        // logger show Key and Value
        foreach (var item in rec)
        {
            _logger.LogInformation($"Key: {item.Key}, Value: {item.Value}");
            list_messages.Add(item.Value);
        }
        
        // convert to json format
        var json = JsonConvert.SerializeObject(list_messages);

        //return rec;
        return json;
    }

    [HttpGet]
    [Route("test/add-attempts/{user}")]
    public async Task<JsonResult> AddAttempt(Message message)
    {
        //var rec = await service.GetRecordAsync(user).ConfigureAwait(false);
        await service.PutRecordAsync(message.GetUUID(), message.GetMessages()).ConfigureAwait(false);
        var newCount = await service.GetRecordAsync(message.GetUUID()).ConfigureAwait(false);
        return new JsonResult("new count: " + newCount);
    }
}