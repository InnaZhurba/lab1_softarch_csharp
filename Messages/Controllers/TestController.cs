using System.Collections.Immutable;
using System.Collections.Specialized;
using Messages.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Messages.Services;

namespace Messages.Controllers;

public class TestController
{
    private readonly IHazelcastService<string> service;
    private readonly ILogger _logger;

    public TestController(ILoggerFactory loggerFactory, Services.IHazelcastService<string> _service)
    {
        service = _service;
        _logger = loggerFactory.CreateLogger<MessagesController>();
    }

    [HttpGet]
    [Route("test/get-attempt/{id}")]
    public async Task<JsonResult> GetAttempt()
    {
        var rec = await service.GetRecordAsync().ConfigureAwait(false);
        
        return new JsonResult(rec);
    }
    
    [HttpGet]
    [Route("test/get-all-attempts")]
    public async Task<string> GetAttempts()
    //public async Task<IReadOnlyDictionary<string,int>> GetAttempts()
    {
        var rec = await service.GetRecordsAsync().ConfigureAwait(false);
        Queue<string> queue_messages = new Queue<string>();
        
        // copy rec to queue_messages
        foreach (var item in rec)
        {
            queue_messages.Enqueue(item);
        }
        
        // convert to json format
        var json = JsonConvert.SerializeObject(queue_messages);

        //return rec;
        return json;
    }

    [HttpGet]
    [Route("test/add-attempts/{user}")]
    public async Task<JsonResult> AddAttempt(Message message)
    {
        _logger.LogInformation($"message testController->addAttempt: {message.GetMessages()}");
        //var rec = await service.GetRecordAsync(user).ConfigureAwait(false);
        await service.PutRecordAsync(message.GetMessages()).ConfigureAwait(false);
        var newCount = await service.GetRecordAsync().ConfigureAwait(false);
        return new JsonResult("new count: " + newCount);
    }
}