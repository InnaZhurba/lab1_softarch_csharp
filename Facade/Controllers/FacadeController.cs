using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Facade.Controllers;

public class FacadeController
{
    
    // facade service send requests to logging and messages services
    // it get requests from client service like POST/GET

    // created WebClient`s for logging and messages services
    private WebClient loggingService = new WebClient();
    private WebClient messagesService = new WebClient();
    
    // URIs of logging and messages services
    string loggingURI = "http://localhost:5248/";
    string messagesURI = "http://localhost:5133/";
    
    // Logger
    //private static readonly ILogger<FacadeController> _logger;
    private readonly ILogger _logger;

    public FacadeController(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FacadeController>();
    }
    
    // GET request
    public string GetMessages()
    {
        //message: get messages from MessagesController
        _logger.LogInformation("Get messages from MessagesController");
        var messages =  messagesService.DownloadString(messagesURI+"get");
        var logging = loggingService.DownloadString(loggingURI+"get");
        
        /*
        logging = logging.TrimStart('\"');
        logging = logging.TrimEnd('\"');
        logging = logging.Replace("\\", "");
        */

        //convert json to string format
        // using nuget package Newtonsoft.Json for convert json to string format
        messages = JsonConvert.DeserializeObject<string>(messages);

        //convert json to List<string> format
        // using nuget package Newtonsoft.Json for convert json to List<string> format
        var new_logging = JsonConvert.DeserializeObject<string>(logging);

        return new_logging + " : " +  messages;
    }
    
    // POST request
    public void AddMessage(string message)
    {
        //message: added to LoggingController
        _logger.LogInformation($"Message added to LoggingController: {message}");

        //convert string to json format
        var json = new StringContent(message, Encoding.UTF8, "application/json");

        //send message to logging service in json format 
        // logging service is the other web microservice in other project
        loggingService.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        
        // using nuget package Newtonsoft.Json for convert string to json format 
        loggingService.UploadString(loggingURI + "post", JsonConvert.ToString(message));
    }
}