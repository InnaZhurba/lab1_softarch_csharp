using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;
using System.IO;
using Newtonsoft.Json.Linq;

public class ServiceRegistrationHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;

    public ServiceRegistrationHostedService(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Отримання конфігурації сервісу (наприклад, адреси, порту, тощо)
        string serviceID = "message-service1";
        string serviceName = "MessageService";
        string serviceAddress = "localhost";
        int servicePort = 5133;
        
        string adress_port = "http://" + serviceAddress + ":" + servicePort;

        var registration = new AgentServiceRegistration
        {
            ID = serviceID,
            Name = serviceName,
            Address = serviceAddress,
            Port = servicePort
        };

        // Реєстрація сервісу в Consul
        _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        
        ChangeLaunchConfig(adress_port); // Зміна конфігурації сервісу (launchSettings.json)

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // код при зупинці сервісу (якщо потрібно)
        return Task.CompletedTask;
    }

    private void ChangeLaunchConfig(string adress_port)
    {
        string launchSettingsPath = "../Properties/launchSettings.json"; // Шлях до файлу launchSettings.json

        // Зчитування вмісту файлу launchSettings.json
        string launchSettingsJson = File.ReadAllText(launchSettingsPath);

        // Парсинг JSON
        JObject launchSettings = JObject.Parse(launchSettingsJson);

        // Заміна значення applicationUrl
        JObject profiles = (JObject)launchSettings["profiles"];
        JObject httpProfile = (JObject)profiles["http"];
        httpProfile["applicationUrl"] = adress_port; // Нове значення applicationUrl

        // Запис модифікованого JSON назад у файл
        File.WriteAllText(launchSettingsPath, launchSettings.ToString());
    }
}