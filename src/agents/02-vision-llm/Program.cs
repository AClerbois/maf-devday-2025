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
    .CreateAIAgent(
        name: "VisionAgent",
        instructions: "Vous êtes un agent utile capable d'analyser des images.");


// Création d'un message combinant du texte et une image
ChatMessage message = new(ChatRole.User, [
    new TextContent("Que voyez-vous dans cette image ?"),
    new UriContent("https://devday.be/assets/gallery-12-C7ubMmX1.jpg", "image/jpeg")
]);

// ChatMessage contient 4 propriétés :
// - Assistant : Obtient le rôle qui fournit des réponses aux entrées de l'utilisateur et aux instructions du système.
// - User : Représente l'utilisateur qui interagit avec l'agent.
// - System : Contient des instructions ou des informations contextuelles pour l'agent.
// - Tool : Obtient le rôle qui fournit des informations supplémentaires et des références en réponse aux demandes d'utilisation des outils.


// Exécution de l'agent avec le message combiné
Console.WriteLine(await agent.RunAsync(message));