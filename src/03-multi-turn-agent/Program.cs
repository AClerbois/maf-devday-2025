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
        .GetChatClient("gpt-4o")
        .CreateAIAgent(instructions: "Tu es doué pour raconter des blagues sarcastiques.", name: "Joker");

AgentThread thread = agent.GetNewThread();

Console.WriteLine(await agent.RunAsync("Raconte une blague une blague au sujet des pirates.", thread));
Console.WriteLine();
Console.WriteLine("--- Nouvelle requête dans le même thread ---");
Console.WriteLine();
Console.WriteLine(await agent.RunAsync("Maintenant, ajoute quelques émojis à la blague et raconte-la avec la voix d'un perroquet de pirate.", thread));