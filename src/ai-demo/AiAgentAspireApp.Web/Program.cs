using AiAgentAspireApp.Web;
using AiAgentAspireApp.Web.Components;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.AddAzureOpenAIClient("chat-demo", settings =>
{
    settings.EnableSensitiveTelemetryData = true;
})
   .AddChatClient();

builder.Services.AddSingleton<McpClient>(sp =>
{
    McpClientOptions mcpClientOptions = new()
    { ClientInfo = new() { Name = "AspNetCoreSseClient", Version = "1.0.0" } };

    HttpClient httpClient = new()
    {
        BaseAddress = new(Environment.GetEnvironmentVariable("DEVDAYCONTENTMCP_https") + "/sse")
    };

    var transport = new HttpClientTransport(new()
    {
        Endpoint = new Uri(Environment.GetEnvironmentVariable("DEVDAYCONTENTMCP_https") + "/sse"),
        Name = "DevDay Content Client",

    }, httpClient);

    var client = McpClient.CreateAsync(transport).GetAwaiter().GetResult();

    var tools = client.ListToolsAsync().GetAwaiter().GetResult();
    if (tools.Count == 0)
    {
        Console.WriteLine("No tools available on the server.");
    }
    else
    {

        Console.WriteLine($"Found {tools.Count} tools on the server.");
        Console.WriteLine();
    }

    return client;
});

builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new("https+http://apiservice"));

builder.Services.AddHttpClient<AgentApiClient>(client => client.BaseAddress = new("https+http://apiservice"));

builder.Services.AddHttpClient<ProductApiClient>(client => client.BaseAddress = new("https+http://apiservice"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
