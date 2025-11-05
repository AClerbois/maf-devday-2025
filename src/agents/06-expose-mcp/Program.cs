using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(
            instructions: @"
                Tu es un agent qui fourni le nom d'une bière belge à chaque requête que tu reçois. 
                Evite les bières clichées comme la Leffe ou la Chimay.
                Réponds uniquement avec le nom de la bière, sans autre texte.",
            name: "BeerBot");


McpServerTool tool = McpServerTool.Create(agent.AsAIFunction());


HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools([tool]);

await builder.Build().RunAsync();