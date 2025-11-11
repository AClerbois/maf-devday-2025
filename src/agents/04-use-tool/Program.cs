using System.ComponentModel;
using Maf;
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(
            instructions: "Tu es un assistant utile qui fournit des informations sur les intervenants de DevDay 2025.",
            tools: [AIFunctionFactory.Create(SpeakerTools.GetSpeakerByName)]);

System.Console.WriteLine("Q: Quelle session Adrien Clerbois présente-t-il ?");

Console.WriteLine(await agent.RunAsync("Quelle session Adrien Clerbois présente-t-il ?"));
System.Console.WriteLine("");
System.Console.WriteLine("-----");
System.Console.WriteLine("");
System.Console.WriteLine("Q: Quelle session Renaud Dumont présente-t-il ?");
Console.WriteLine(await agent.RunAsync("Quelle session Renaud Dumont présente-t-il ?"));