using System.Collections.Specialized;
using Logging.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Logging.Controllers;

public class LoggingController
{
    
    // logger 
    //private static readonly ILogger<LoggingController> _logger;
    private readonly ILogger _logger;
    
    // Map for saving UUID and message
    private List<Message> messages = new List<Message>();


    public LoggingController(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LoggingController>();
    }
   
    public void AddMessage(string message)
    {
        // add message to objects
        messages.Add(new Message(message));

        //message: added to LoggingController
        _logger.LogInformation($"Message added to LoggingController: {message}");
    }

    public string GetMessages()
    {
        // message: get messages from LoggingController
        _logger.LogInformation("Get messages from LoggingController");
        
        // get all string messages from objects and add to List<string>
        List<string> list_messages = new List<string>();
        foreach (var message in this.messages)
        {
            list_messages.Add(message.GetMessages());
        }
        
        // convert to json format
        // using nuget package Newtonsoft.Json for convert List<string> to json format
        var json = JsonConvert.SerializeObject(list_messages);
        return json; //messages;
    }
}