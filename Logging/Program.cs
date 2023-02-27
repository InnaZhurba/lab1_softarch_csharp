using Logging.Controllers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
LoggingController loggingController = new LoggingController(builder.Services.BuildServiceProvider().GetService<ILoggerFactory>());

// post request to save message to logging service from http request body
app.MapPost("/post",  async (HttpRequest request) =>
{
    string message = request.ReadFromJsonAsync(typeof(string)).ToString();
    
    ILogger logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Logging");
    logger.LogInformation($"message logging->program: {message}");
    
    loggingController.AddMessage(message);
    
    return Results.Ok();
});


//send get request to get all messages from logging service
app.MapGet("/get", () => {
    return Results.Ok(loggingController.GetMessages());
});

app.Run();