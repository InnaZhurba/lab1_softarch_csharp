using Hazelcast;
using Logging.Controllers;
using Logging.Domain;
using Logging.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var options = new HazelcastOptions();
options.ClusterName = "dev";
options.ClientName = "net-client";
options.Networking.Addresses.Add("localhost:5701");

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
    
    ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logging");
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