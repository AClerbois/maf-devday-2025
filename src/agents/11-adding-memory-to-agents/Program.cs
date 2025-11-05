using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;
using OpenAI;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

ChatClient chatClient = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o");

AIAgent agent = chatClient.CreateAIAgent(new ChatClientAgentOptions()
{
    Instructions = @"Vous êtes un assistant sympathique. 
        Adressez-vous toujours à l'utilisateur par son nom.",
    AIContextProviderFactory = ctx => new UserInfoMemory(
        chatClient.AsIChatClient(),
        ctx.SerializedState,
        ctx.JsonSerializerOptions)
});


// Create a new thread for the conversation.
AgentThread thread = agent.GetNewThread();

System.Console.WriteLine("--- Conversation with Memory Enabled Agent ---");
System.Console.WriteLine("--- Q: Bonjour, quelle est la racine carrée de 9 ? ---");

Console.WriteLine(await agent.RunAsync("Bonjour, quelle est la racine carrée de 9 ?", thread));
System.Console.WriteLine("--- Q: Je m'appelle Adrien ---");
Console.WriteLine(await agent.RunAsync("Je m'appelle Adrien", thread));

System.Console.WriteLine("--- Q: J'ai 35 ans ---");
Console.WriteLine(await agent.RunAsync("J'ai 35 ans", thread));

// Access the memory component via the thread's GetService method.
var userInfo = thread.GetService<UserInfoMemory>()?.UserInfo;
Console.WriteLine($"MEMORY - User Name: {userInfo?.UserName}");
Console.WriteLine($"MEMORY - User Age: {userInfo?.UserAge}");