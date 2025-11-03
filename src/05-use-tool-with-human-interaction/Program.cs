using System.ComponentModel;
using Maf;
using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;


AIFunction getSpeakerFunction = AIFunctionFactory.Create(SpeakerTools.GetSpeakerByName);
AIFunction approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(getSpeakerFunction);


AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(
            instructions: "Tu es un assistant utile qui fournit des informations sur les intervenants de DevDay 2025.",
            tools: [approvalRequiredWeatherFunction]);

Console.WriteLine("Initial request:");

AgentThread thread = agent.GetNewThread();
AgentRunResponse response = await agent.RunAsync("Quelle session Adrien Clerbois présente-t-il ?", thread);

var functionApprovalRequests = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<FunctionApprovalRequestContent>()
    .ToList();

FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();

Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");


var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
Console.WriteLine(await agent.RunAsync(approvalMessage, thread));