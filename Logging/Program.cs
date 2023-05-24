using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Hazelcast;
using Logging.Controllers;
using Logging.Domain;
using Logging.Services;
using Microsoft.AspNetCore.Mvc;
using Consul;

Process.Start("/Users/innazhurba/Documents/06semesterUCU/software_architecture/lab1/lab1_softarch_coreweb/Logging/hazelcast-5.2.2/bin/hz-start");

// ------------------------------------------
// Реєстрація ConsulClient
// Отримання конфігурації сервісу (наприклад, адреси, порту, тощо)
string serviceID = "logging-service1";
string serviceName = "LoggingService";
string serviceAddress = "localhost";
int servicePort = 5247;
        
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


//var consulConfig = new ConsulClientConfiguration { Address = new Uri("http://localhost:8500") };

var keyValuePair = await consulClient.KV.Get("hazelcast-config");
var hazelcastConfigJson = Encoding.UTF8.GetString(keyValuePair.Response.Value);


var builder = WebApplication.CreateBuilder(args);
ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logging");

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

logger.LogInformation($"logging->program: get hazelcast options from json file");

builder.Services.AddSingleton<IHazelcastService<int, string>, HazelcastService<int, string>>
    ((service => new HazelcastService<int, string>(options, "logging_messages")));

var app = builder.Build();


LoggingController loggingController = new LoggingController(builder.Services.BuildServiceProvider().GetService<ILoggerFactory>());
TestController testController = new TestController(builder.Services.BuildServiceProvider().GetService<ILoggerFactory>(),
    builder.Services.BuildServiceProvider().GetService<IHazelcastService<int,string>>()); 

// post request to save message to logging service from http request body
app.MapPost("/post",  async (HttpRequest request) =>
{
    string message = request.ReadFromJsonAsync(typeof(string)).ToString();
    
    logger.LogInformation($"message logging->program: {message}");
    
    // old version (lab1)
    //loggingController.AddMessage(message);
    
    // new version (lab3)
    // + add UUID to message
    // + create Message object
    Message message_obj = new Message(message);
    
    await testController.AddAttempt(message_obj);
    
    return Results.Ok();
});


//send get request to get all messages from logging service
app.MapGet("/get", async() =>
{
    // added in lab3
    return Results.Ok(await testController.GetAttempts());
    
    // old version (lab1)
    //return Results.Ok(loggingController.GetMessages());
});

app.Run();