using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI;
using System.IO;
using System.Text.Json;


AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential()
    )
    .GetChatClient("gpt-4o")
    .CreateAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");

AgentThread thread = agent.GetNewThread();

// Run the agent and append the exchange to the thread
Console.WriteLine(await agent.RunAsync("Tell me a short pirate joke.", thread));


// Serialize the thread state
string serializedJson = thread.Serialize(JsonSerializerOptions.Web).GetRawText();

// Example: save to a local file (replace with DB or blob storage in production)
string filePath = Path.Combine("agent_thread.json");
await File.WriteAllTextAsync(filePath, serializedJson);



// Read persisted JSON
string loadedJson = await File.ReadAllTextAsync(filePath);
JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);

// Deserialize the thread into an AgentThread tied to the same agent type
AgentThread resumedThread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);


// Continue the conversation with resumed thread
Console.WriteLine(await agent.RunAsync("Now tell that joke in the voice of a pirate.", resumedThread));