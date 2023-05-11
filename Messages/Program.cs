using System.Diagnostics;
using Hazelcast;
using Messages.Controllers;
using Messages.Domain;
using Messages.Services;

//var builder = WebApplication.CreateBuilder(args);

Process.Start("/Users/innazhurba/Documents/06semesterUCU/software_architecture/lab1/lab1_softarch_coreweb/Messages/hazelcast-5.2.2/bin/hz-start");

var builder = WebApplication.CreateBuilder(args);

var options = new HazelcastOptions();
options.ClusterName = "dev";
options.ClientName = "net-client";

builder.Services.AddSingleton<IHazelcastService<string>, HazelcastService<string>>
    ((service => new HazelcastService<string>(options, "messages")));

ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Massaging");


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