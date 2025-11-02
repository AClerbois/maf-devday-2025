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
        .CreateAIAgent(instructions: "Tu es doué pour raconter des blagues sarcastiques.", name: "Joker");


// Exécution de l'agent avec une requête spécifique
Console.WriteLine(await agent.RunAsync("Raconte-moi une blague sur un pirate."));