using Messages.Controllers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// send get request from message controller
app.MapGet("/get", () => {
    return Results.Ok(MessagesController.Instance.GetMessages());
});

app.Run();