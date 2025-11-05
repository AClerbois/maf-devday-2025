using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;

AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
        .CreateAIAgent(new ChatClientAgentOptions
        {
            Name = "Joker",
            Instructions = "You are good at telling jokes.",
            ChatMessageStoreFactory = ctx =>
            {
                // Create a new chat message store for this agent that stores the messages in a vector store.
                return new VectorChatMessageStore(
                   new InMemoryVectorStore(),
                   ctx.SerializedState,
                   ctx.JsonSerializerOptions);
            }
        });

System.Console.WriteLine(await agent.RunAsync("Tell me a funny joke."));



