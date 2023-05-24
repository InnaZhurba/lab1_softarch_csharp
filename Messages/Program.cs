using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Hazelcast;
using Messages.Controllers;
using Messages.Domain;
using Messages.Services;
using Consul;

//var builder = WebApplication.CreateBuilder(args);
Process.Start("/Users/innazhurba/Documents/06semesterUCU/software_architecture/lab1/lab1_softarch_coreweb/Messages/hazelcast-5.2.2/bin/hz-start");

// ------------------------------------------
// Реєстрація ConsulClient
// Отримання конфігурації сервісу (наприклад, адреси, порту, тощо)
string serviceID = "message-service2";
string serviceName = "MessageService";
string serviceAddress = "localhost";
int servicePort = 5134;
        
string adress_port = "http://" + serviceAddress + ":" + servicePort;

var registration = new AgentServiceRegistration
{
    ID = serviceID,
    Name = serviceName,
    Address = serviceAddress,
    Port = servicePort
};

var consulClient = new ConsulClient();
var result = consulClient.Agent.ServiceRegister(registration).Result;


// ------------------------------------------
// GET hazelcast-config.json
//var consulConfig = new ConsulClientConfiguration { Address = new Uri("http://localhost:8500") };


var keyValuePair = await consulClient.KV.Get("hazelcast-config");
var hazelcastConfigJson = Encoding.UTF8.GetString(keyValuePair.Response.Value);

// ------------------------------------------
// get logger from builder
var builder = WebApplication.CreateBuilder(args);

ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Massaging");

// ------------------------------------------
// get hazelcast options from json file
string jsonString = File.ReadAllText(hazelcastConfigJson);
var jsoptions = JsonSerializer.Deserialize<HazelcastOptions>(jsonString);

// check if options are correct
if (jsoptions == null)
{
    Console.WriteLine("Error: can't read hazelcast-config.json");
    return;
}

var options = new HazelcastOptions();
options.ClusterName = jsoptions.ClusterName;
options.ClientName = jsoptions.ClientName;

logger.LogInformation($"message message_service->program: get hazelcast options from json file");

builder.Services.AddSingleton<IHazelcastService<string>, HazelcastService<string>>
    ((service => new HazelcastService<string>(options, "messages")));


var app = builder.Build();

TestController testController = new TestController(builder.Services.BuildServiceProvider().GetService<ILoggerFactory>(),
    builder.Services.BuildServiceProvider().GetService<IHazelcastService<string>>()); 

// send get request from message controller
app.MapGet("/get", async() =>
{
    return Results.Ok(await testController.GetAttempts());
    //return Results.Ok(MessagesController.Instance.GetMessages());
});

// post request to save message
app.MapPost("/post",  async (HttpRequest request) =>
{
    string message = request.ReadFromJsonAsync(typeof(string)).ToString();
    
    ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logging");
    logger.LogInformation($"message message_service->program: {message}");
    
    // old version (lab1)
    //loggingController.AddMessage(message);
    
    // new version (lab3)
    // + add UUID to message
    // + create Message object
    Message message_obj = new Message(message);
    
    await testController.AddAttempt(message_obj);
    
    return Results.Ok();
});


app.Run();