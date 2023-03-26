using System.Net;
using Facade.Controllers;
using Facade.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

FacadeService facadeService = new FacadeService(builder.Services.BuildServiceProvider().GetService<ILoggerFactory>());

// facade service send requests to logging and messages services
// it get requests from client service like POST/GET
var facadeController = new FacadeController( builder.Services.BuildServiceProvider().GetService<ILoggerFactory>(), facadeService);

//send post request to save message to loggingservice from http request body
app.MapPost("/post", async (HttpRequest request) =>
{
    
    string message = request.ReadFromJsonAsync(typeof(string)).ToString();
    facadeController.AddMessage(message);
    
    return Results.Ok();
});

//send get request to get all messages from loggingservice
app.MapGet("/get", () => {
    return Results.Ok(facadeController.GetMessages());
});

app.Run();