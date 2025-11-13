using System.Text;
using System.Text.Json;
using AiAgentAspireApp.Web.Workflows;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.AI;
using Microsoft.JSInterop;
using ModelContextProtocol.Client;

namespace AiAgentAspireApp.Web.Components.Pages;

public partial class AiChat
{
    private string userInput = string.Empty;
    private ElementReference messagesContainer;
    private readonly List<ChatMessageModel> Messages = [];
    private bool isStreaming = false;

    [Inject] public required McpClient McpClient { get; set; }
    [Inject] public required IChatClient ChatClientInjected { get; set; }
    [Inject] private IJSRuntime? JSRuntime { get; set; }
    [Inject] private ILogger<AiChat> Logger { get; set; } = default!;

    private class ChatMessageModel
    {
        public string Content { get; set; } = string.Empty;
        public bool IsUser { get; set; }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(userInput) || isStreaming)
        {
            return;
        }

        Logger.LogInformation("User submitted query: {Query}", userInput);

        // Add user message
        Messages.Add(new ChatMessageModel
        {
            Content = userInput,
            IsUser = true
        });

        var currentUserInput = userInput;
        userInput = string.Empty;

        // Prepare AI response container
        var aiResponse = new StringBuilder();

        Messages.Add(new ChatMessageModel
        {
            Content = "🔍 Recherche en cours...",
            IsUser = false
        });

        var workflowStartTime = DateTime.UtcNow;

        try
        {
            isStreaming = true;
            await InvokeAsync(StateHasChanged);

            Logger.LogInformation("Building LinkedIn Content Workflow for query: {Query}", currentUserInput);

            var aiChat = new AzureOpenAIClient(
                new Uri("https://devday-2025-maf.openai.azure.com/"),
                new AzureCliCredential())
                .GetChatClient("gpt-4o").AsIChatClient();

            // Créer et construire le workflow
            var workflowBuilder = new LinkedInContentWorkflow(aiChat, McpClient,
                Logger as ILogger<LinkedInContentWorkflow> ?? LoggerFactory.Create(b => b.AddConsole()).CreateLogger<LinkedInContentWorkflow>());
            var workflow = await workflowBuilder.BuildWorkflowAsync();

            Logger.LogInformation("Starting workflow execution");

            // Exécuter le workflow avec streaming
            StreamingRun run = await InProcessExecution.StreamAsync(
                workflow,
                new ChatMessage(ChatRole.User, currentUserInput));

            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

            string currentStage = "Initialisation";
            bool foundContent = true;
            int eventCount = 0;

            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                eventCount++;
                Logger.LogDebug("Workflow event #{Count}: {EventType}", eventCount, evt.GetType().Name);

                switch (evt)
                {
                    case ExecutorInvokedEvent invokedEvent:
                        currentStage = GetStageDescription(invokedEvent.ExecutorId);
                        Logger.LogInformation("Executor started: {ExecutorId} - {Stage}", invokedEvent.ExecutorId, currentStage);
                        Messages[^1].Content = $"⚙️ {currentStage}...";
                        await InvokeAsync(StateHasChanged);
                        await ScrollToBottom();
                        break;

                    case ExecutorCompletedEvent completedEvent:
                        Logger.LogInformation("Executor completed: {ExecutorId}", completedEvent.ExecutorId);

                        // Check if MCP search found nothing
                        if (completedEvent.ExecutorId == "McpSearchExecutor" && completedEvent.Data != null)
                        {
                            var mcpResult = completedEvent.Data as McpSearchResult;
                            if (mcpResult?.Found == false)
                            {
                                Logger.LogWarning("Workflow stopped: No content found for query '{Query}'", currentUserInput);
                                foundContent = false;
                                Messages[^1].Content = $"❌ {mcpResult.Message}";
                                await InvokeAsync(StateHasChanged);
                                return;
                            }
                        }
                        break;

                    case AgentRunUpdateEvent updateEvent:
                        if (updateEvent.Data != null)
                        {
                            Logger.LogDebug("Agent update from {ExecutorId}: {DataLength} chars",
                                updateEvent.ExecutorId, updateEvent.Data.ToString()?.Length ?? 0);
                            aiResponse.Append(updateEvent.Data.ToString());
                            Messages[^1].Content = $"⚙️ {currentStage}...\n\n{aiResponse}";
                            await InvokeAsync(StateHasChanged);
                            await ScrollToBottom();
                        }
                        break;

                    case WorkflowOutputEvent outputEvent:
                        Logger.LogInformation("Workflow output received");

                        if (outputEvent.Data != null && foundContent)
                        {
                            // Try to extract BilingualContent from output
                            var bilingualContent = ExtractBilingualContent(outputEvent.Data);
                            if (bilingualContent != null)
                            {
                                Logger.LogInformation("Successfully extracted bilingual content. FR: {FrLength} chars, EN: {EnLength} chars",
                                    bilingualContent.French?.Length ?? 0, bilingualContent.English?.Length ?? 0);
                                var finalContent = FormatBilingualContent(bilingualContent);
                                Messages[^1].Content = finalContent;
                            }
                            else
                            {
                                Logger.LogWarning("Failed to extract bilingual content, using raw output");
                                var output = outputEvent.Data.ToString() ?? "";
                                Messages[^1].Content = $"✅ Contenu créé :\n\n{output}";
                            }
                        }
                        await InvokeAsync(StateHasChanged);
                        await ScrollToBottom();
                        break;

                    case WorkflowErrorEvent errorEvent:
                        Logger.LogError("Workflow error occurred");
                        Messages[^1].Content = $"❌ Une erreur s'est produite lors du traitement du workflow.";
                        await InvokeAsync(StateHasChanged);
                        break;
                }
            }

            var workflowDuration = DateTime.UtcNow - workflowStartTime;
            Logger.LogInformation("Workflow completed successfully in {Duration}ms. Total events: {EventCount}",
                workflowDuration.TotalMilliseconds, eventCount);
        }
        catch (Exception ex)
        {
            var workflowDuration = DateTime.UtcNow - workflowStartTime;
            Logger.LogError(ex, "Workflow failed after {Duration}ms for query: {Query}",
                workflowDuration.TotalMilliseconds, currentUserInput);
            Messages[^1].Content = $"❌ Erreur : {ex.Message}";
        }
        finally
        {
            isStreaming = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private string GetStageDescription(string executorId)
    {
        return executorId switch
        {
            "McpSearchExecutor" => "Recherche dans les données DevDay",
            "BingSearchExecutor" => "Recherche d'informations complémentaires (par défaut, sauf si 'sans recherche internet')",
            "StoryCreatorExecutor" => "Création de l'histoire LinkedIn",
            "EditorExecutor" => "Édition et polissage du contenu",
            "TranslatorExecutor" => "Traduction en français et anglais",
            _ => "Traitement en cours"
        };
    }

    private BilingualContent? ExtractBilingualContent(object data)
    {
        try
        {
            // Si c'est déjà un BilingualContent, le retourner directement
            if (data is BilingualContent bilingual)
            {
                return bilingual;
            }

            var jsonString = data.ToString() ?? "";

            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            // Nettoyer le JSON si c'est dans un code block markdown
            if (jsonString.Contains("```json") || jsonString.Contains("```"))
            {
                Logger.LogDebug("Cleaning markdown code blocks from bilingual content");
                var lines = jsonString.Split('\n');
                var jsonLines = new List<string>();
                bool inCodeBlock = false;

                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("```"))
                    {
                        inCodeBlock = !inCodeBlock;
                        continue;
                    }
                    if (inCodeBlock || (!line.Trim().StartsWith("```") && (line.Contains("{") || line.Contains("}"))))
                    {
                        jsonLines.Add(line);
                    }
                }

                jsonString = string.Join("\n", jsonLines);
            }

            // Essayer d'extraire juste l'objet JSON s'il y a du texte autour
            var startIndex = jsonString.IndexOf('{');
            var endIndex = jsonString.LastIndexOf('}');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                jsonString = jsonString.Substring(startIndex, endIndex - startIndex + 1);
            }

            var result = JsonSerializer.Deserialize<BilingualContent>(jsonString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                });

            // Vérifier que les deux propriétés sont bien remplies
            if (result != null && !string.IsNullOrWhiteSpace(result.French) && !string.IsNullOrWhiteSpace(result.English))
            {
                return result;
            }
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "JSON parsing error while extracting bilingual content");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error extracting bilingual content");
        }

        return null;
    }

    private string FormatBilingualContent(BilingualContent content)
    {
        var formatted = new StringBuilder();
        formatted.AppendLine("# 🎯 Contenu LinkedIn Prêt à Publier");
        formatted.AppendLine();
        formatted.AppendLine("---");
        formatted.AppendLine();
        formatted.AppendLine("## 🇫🇷 Version Française");
        formatted.AppendLine();
        formatted.AppendLine(content.French);
        formatted.AppendLine();
        formatted.AppendLine("---");
        formatted.AppendLine();
        formatted.AppendLine("## 🇬🇧 English Version");
        formatted.AppendLine();
        formatted.AppendLine(content.English);
        formatted.AppendLine();
        formatted.AppendLine("---");

        return formatted.ToString();
    }

    private async Task ScrollToBottom()
    {
        if (JSRuntime != null)
        {
            await JSRuntime.InvokeVoidAsync("scrollToBottom", messagesContainer);
        }
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !isStreaming)
        {
            await SendMessage();
        }
    }
}
