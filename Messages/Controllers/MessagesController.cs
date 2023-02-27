namespace Messages.Controllers;

public class MessagesController
{
    // Static message for GET request
    private static string message = "messages-service is not implemented yet";
    
    // singleton instance
    private static MessagesController instance;
    private static readonly object padlock = new object();

    // Logger 
    //private static readonly ILogger<MessagesController> _logger;
    
    //implement 
    private MessagesController()
    {
    }
    
    public static MessagesController Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new MessagesController();
                }
                //creation of messages controller instance
                //_logger.LogInformation("MessagesController instance created");
                
                return instance;
            }
        }
    }

    // GET request
    public string GetMessages()
    {
        //message: get messages from MessagesController
        //_logger.LogInformation("Get messages from MessagesController");
        return message;
    }
}