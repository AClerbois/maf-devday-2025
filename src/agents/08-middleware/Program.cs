using System;
using System.ComponentModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("--- Agent Middleware Example ---");

[Description("The current datetime offset.")]
static string GetDateTime()
    => DateTimeOffset.Now.ToString();

AIAgent baseAgent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(
            instructions: "You are an AI assistant that helps people find information.",
            tools: [AIFunctionFactory.Create(GetDateTime, name: nameof(GetDateTime))]);


async Task<AgentRunResponse> CustomAgentRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Input: {messages.Count()}");
    var response = await innerAgent.RunAsync(messages, thread, options, cancellationToken).ConfigureAwait(false);
    Console.WriteLine($"Output: {response.Messages.Count}");
    return response;
}


var middlewareEnabledAgent = baseAgent
    .AsBuilder()
        .Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
    .Build();

Console.WriteLine(await middlewareEnabledAgent.RunAsync("What's the current time?"));



Console.WriteLine("--- ChatClient Middleware Example ---");



var chatClient = new AzureOpenAIClient(
        new Uri("https://devday-2025-maf.openai.azure.com/"),
        new AzureCliCredential()
    )
    .GetChatClient("gpt-4o")
    .AsIChatClient();

var middlewareEnabledChatClient = chatClient
    .AsBuilder()
    .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
    .Build();

var agent = new ChatClientAgent(
    middlewareEnabledChatClient,
    instructions: "You are a helpful assistant.");

async Task<ChatResponse> CustomChatClientMiddleware(
    IEnumerable<ChatMessage> messages,
    ChatOptions? options,
    IChatClient innerChatClient,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Input: {messages.Count()}");
    var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
    Console.WriteLine($"Output: {response.Messages.Count}");

    return response;
}

Console.WriteLine(await agent.RunAsync("What's the current time?"));
