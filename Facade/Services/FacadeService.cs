using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Facade.Services;

public class FacadeService
{
    private readonly ILogger _logger;
    
    // created WebClient`s for logging and messages services
    private WebClient loggingService = new WebClient();
    private WebClient messagesService = new WebClient();
    
    // URIs of logging and messages services
    //string loggingURI = "http://localhost:5248/";
    //private string messagesURI = "http://localhost:5133/";
    
    // list of loggingURLs with URLS : "http://localhost:5248/", "http://localhost:5247/", http://localhost:5249/"
    private static string[] loggingURLs = {"http://localhost:5248/", "http://localhost:5247/", "http://localhost:5249/"};
    
    // list of messagesURLs with URLS : "http://localhost:5133/", "http://localhost:5132/", http://localhost:5134/"
    private static string[] messagesURLs = {"http://localhost:5133/", "http://localhost:5134/"};


    public FacadeService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FacadeService>();
    }
    
    // GET request
    public string GetMessages()
    {
        //choose random loggingURL from loggingURLs
        string loggingURI = loggingURLs[new Random().Next(0, loggingURLs.Length)];
        
        //choose random messagesURL from messagesURLs
        string messagesURI = messagesURLs[new Random().Next(0, messagesURLs.Length)];
        
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
        //choose random loggingURL from loggingURLs
        string loggingURI = loggingURLs[new Random().Next(0, loggingURLs.Length)];
        
        //choose random messagesURL from messagesURLs
        string messagesURI = messagesURLs[new Random().Next(0, messagesURLs.Length)];
        
        //message: added to LoggingController
        _logger.LogInformation($"Message added to LoggingController: {message}");

        //convert string to json format
        var json = new StringContent(message, Encoding.UTF8, "application/json");

        //send message to logging service in json format 
        // logging service is the other web microservice in other project
        loggingService.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        
        // using nuget package Newtonsoft.Json for convert string to json format 
        loggingService.UploadString(loggingURI + "post", JsonConvert.ToString(message));
        
        //send message to messages service in json format
        // messages service is the other web microservice in other project
        messagesService.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        
        // using nuget package Newtonsoft.Json for convert string to json format
        messagesService.UploadString(messagesURI + "post", JsonConvert.ToString(message));
    }
}