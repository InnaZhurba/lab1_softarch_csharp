using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Facade.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Facade.Controllers;

public class FacadeController
{
    private readonly FacadeService _facadeService;
    
    // Logger
    private readonly ILogger _logger;

    public FacadeController(ILoggerFactory loggerFactory, FacadeService facadeService)
    {
        _logger = loggerFactory.CreateLogger<FacadeController>();
        _facadeService = facadeService;
    }
    
    // GET request
    public string GetMessages()
    {
        return _facadeService.GetMessages();
    }
    
    // POST request
    public void AddMessage(string message)
    {
        _facadeService.AddMessage(message);
    }
}