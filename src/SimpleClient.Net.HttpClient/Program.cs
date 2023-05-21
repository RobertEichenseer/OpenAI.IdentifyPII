using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

IHost consoleHost = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddTransient<Main>();
    })
    .Build();

Main main = consoleHost.Services.GetRequiredService<Main>();
await main.ExecuteAsync(args);

class Main
{
    ILogger<Main> _logger;

    public Main (ILogger<Main> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(string[] args)
    {

        var payload = new { 
            prompt = "You are a tool which extracts personal identifiable information (PII) from data provided to you. \nIf you are unsure if there\u0027s PII in the data answer with \u0022The provided dataset needs checking\u0022.\nDo not invent additional information.\nDo not include potential PII information in questions you add to your response.\nExample 1:\nInput: Susanne is a power user and works daily 8 hours with the system\nResponse: xx is a power user and works daily 8 hours with the system\n\nExample 2:\nInput: Charles has used Outlook 15 minutes yesterday\nResponse: xx has used Outlook 15 minutes yesterday\n\nExample 3:\nInput: Robert is driving a car at high speed\nResponse: xx is driving a car at high speed\n\nExample 4:\nInput: Jon has an invitation to a restaurant\nResponse: xx has an invitation to a restaurant\n\nExample 5:\nInput: Olivia is running fast during the competition\nResponse: xx is running fast during the competition\n\n'Ashley has bought a news paper at the airport'",
            max_tokens = 150,
            temperature = (float)0.7,
            top_p=(float)0.95,
            stop="\n",
            presence_penalty=(float)0,
            frequency_penalty=(float)0 
        };
        string jsonPayload = JsonSerializer.Serialize(payload);
        StringContent stringContent = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

        //Please have a look to \src\CreateEnv\CreateEnv where the Environment is created and the variables are set
        //If you don't use the Azure cli script to create the environment please provide the values here
        string endPoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "ENDPOINT-NOT-SET"; //Example: "https://southcentralus.api.cognitive.microsoft.com/"
        string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "API-KEY-NOT-SET";  //Example: "5a5477c4cdea4999ab0ca675259e3530"
        string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENTNAME") ?? "DEPLOYMENTNAME-NOT-SET"; //Example: "gpt35turbo-deployment"
        
        string url = $"{endPoint}openai/deployments/{deploymentName}/completions?api-version=2023-03-15-preview";
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequestMessage.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        httpRequestMessage.Headers.Add("api-key", apiKey); 
        httpRequestMessage.Content = stringContent; 

        using HttpClient httpClient = new HttpClient();
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        string response = await httpResponseMessage.Content.ReadAsStringAsync();

        Console.WriteLine(response);

    }
}

