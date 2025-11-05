using System;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;


// Create a TracerProvider that exports to the console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();


// Create the agent and enable OpenTelemetry instrumentation
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(instructions: @"
                Tu es un agent qui fourni le nom d'une bière à chaque requête que tu reçois. 
                Evite les bières clichées comme la Leffe ou la Chimay.
                Réponds uniquement avec le nom de la bière",
            name: "BeerBot")
        .AsBuilder()
        .UseOpenTelemetry(sourceName: "agent-telemetry-source")
        .Build();

Console.WriteLine(await agent.RunAsync("Donnes une bière de Seattle."));