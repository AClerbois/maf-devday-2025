// Set up the Azure OpenAI client
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using System.ComponentModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System;
using OpenAI;


await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
{
    Name = "MCPServer",
    Command = "npx",
    Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"],
}));

var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);




var agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("gpt-4o")
 .CreateAIAgent(
    instructions: "You answer questions related to GitHub repositories only.",
    tools: [.. mcpTools.Cast<AITool>()]);

System.Console.WriteLine("Q: Summarize the last four issues to the microsoft/agent-framework repository authored by AClerbois?");
// Invoke the agent and output the text result
Console.WriteLine(await agent.RunAsync("Summarize the last four issues to the microsoft/agent-framework repository authored by AClerbois?"));