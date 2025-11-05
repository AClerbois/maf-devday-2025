using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;


// Définition de l'agent AI avec des instructions personnalisées
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("mistral-medium-2505")
        .CreateAIAgent(
            instructions: "Tu es un spécialiste de film popculture.",
            name: "Joker");


// Exécution de l'agent avec une requête spécifique

Console.WriteLine("Q: Raconte-moi un film que tu aimes sur les pirates.");
Console.WriteLine(await agent.RunAsync("Raconte-moi un film que tu aimes sur les pirates."));