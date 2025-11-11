using System;
using System.Threading.Tasks;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;



var model = "gpt-4o";

static async Task<ChatClientAgent> GetTranslationAgentAsync(
       string targetLanguage,
       PersistentAgentsClient persistentAgentsClient,
       string model)
{
    var agentMetadata = await persistentAgentsClient.Administration.CreateAgentAsync(
        model: model,
        name: $"{targetLanguage} Translator",
        instructions: $"You are a translation assistant that translates the provided text to {targetLanguage}.");

    return await persistentAgentsClient.GetAIAgentAsync(agentMetadata.Value.Id);
}

var persistentAgentsClient = new PersistentAgentsClient(
    "https://devday-2025-maf.services.ai.azure.com/api/projects/maf-2025",
     new AzureCliCredential());

AIAgent frenchAgent = null;
AIAgent spanishAgent = null;
AIAgent englishAgent = null;


try
{
    frenchAgent = await GetTranslationAgentAsync("French", persistentAgentsClient, model);
    spanishAgent = await GetTranslationAgentAsync("Spanish", persistentAgentsClient, model);
    englishAgent = await GetTranslationAgentAsync("English", persistentAgentsClient, model);

   // Build the workflow by adding executors and connecting them
    var workflow = new WorkflowBuilder(frenchAgent)
        .AddEdge(frenchAgent, spanishAgent)
        .AddEdge(spanishAgent, englishAgent)
        .Build();


    // Execute the workflow
    await using StreamingRun run = await InProcessExecution.StreamAsync(
        workflow,
         new ChatMessage(ChatRole.User, "Hello World!"));

    // Must send the turn token to trigger the agents.
    // The agents are wrapped as executors. When they receive messages,
    // they will cache the messages and only start processing when they receive a TurnToken.
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
    await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
    {
        System.Console.WriteLine("Event received: " + evt.GetType().Name);

        if (evt is AgentRunUpdateEvent executorComplete)
        {
            Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
        }
    }
}
finally
{
    System.Console.WriteLine("Cleaning up created agents...");
    await persistentAgentsClient.Administration.DeleteAgentAsync(frenchAgent.Id);
    await persistentAgentsClient.Administration.DeleteAgentAsync(spanishAgent.Id);
    await persistentAgentsClient.Administration.DeleteAgentAsync(englishAgent.Id);
}