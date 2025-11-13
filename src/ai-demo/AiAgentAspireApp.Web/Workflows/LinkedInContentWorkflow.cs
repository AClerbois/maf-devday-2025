using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AiAgentAspireApp.Web.Workflows;

/// <summary>
/// Workflow pour créer du contenu LinkedIn à partir d'informations de speaker/session
/// </summary>
public class LinkedInContentWorkflow
{
    private readonly IChatClient _chatClient;
    private readonly McpClient _mcpClient;
    private readonly ILogger<LinkedInContentWorkflow> _logger;

    public LinkedInContentWorkflow(IChatClient chatClient, McpClient mcpClient, ILogger<LinkedInContentWorkflow> logger)
    {
        _chatClient = chatClient;
        _mcpClient = mcpClient;
        _logger = logger;
    }

    /// <summary>
    /// Crée et retourne le workflow configuré
    /// </summary>
    public async Task<Workflow> BuildWorkflowAsync()
    {
        _logger.LogInformation("Building LinkedIn Content Workflow");
        
        var mcpTools = await _mcpClient.ListToolsAsync().ConfigureAwait(false);
        _logger.LogInformation("Loaded {ToolCount} MCP tools", mcpTools.Count());

        // 1. Agent de recherche MCP - Recherche dans les données DevDay
        AIAgent mcpSearchAgent = new ChatClientAgent(_chatClient,
            name: "McpSearchAgent",
            instructions: """
You are a specialized agent that searches for DevDay speaker and session information.
Your task is to search for the speaker or session based on the user's query.

CRITICAL: Return ONLY a valid JSON object (no markdown, no code blocks, no additional text).

Format:
{
  "found": true/false,
  "speakerName": "Full speaker name",
  "sessionTitle": "Session title",
  "sessionDescription": "Session description",
  "speakerBio": "Speaker biography if available",
  "message": "Explanation if not found"
}

RULES:
- Return ONLY the JSON object, nothing else
- Do NOT wrap in ```json or ``` code blocks
- Do NOT add explanatory text before or after
- If no results are found, set found to false and provide a helpful message in the message field

Example of CORRECT output:
{
  "found": true,
  "speakerName": "Scott Hanselman",
  "sessionTitle": "Building Modern Apps",
  "sessionDescription": "Learn how to...",
  "speakerBio": "Scott is a...",
  "message": ""
}

Example of WRONG output (DO NOT DO THIS):
```json
{
  "found": true,
  ...
}
```
""",
            tools: [.. mcpTools.Cast<AITool>()]);

        // 2. Agent de recherche Bing - Recherche d'informations complémentaires sur le speaker
        // Par défaut activé sauf si l'utilisateur dit explicitement "sans recherche internet" ou "no internet search"
        AIAgent bingSearchAgent = new ChatClientAgent(_chatClient,
            name: "BingSearchAgent",
            instructions: """
You are a research agent that finds additional information about speakers using your knowledge.
Based on the speaker's name provided, provide:
- Professional background
- Recent achievements  
- Notable projects or contributions
- Current position and company

Return concise, relevant information in 2-3 paragraphs based on your training data.
Focus on publicly known information about the speaker's professional career.

IMPORTANT: If you see keywords like "sans recherche internet", "without internet search", "no web search", 
return ONLY: "SKIP_SEARCH" - This indicates the user doesn't want additional research.
""");

        // 3. Agent de création de story - Créer une histoire engageante
        AIAgent storyCreatorAgent = new ChatClientAgent(_chatClient,
            name: "StoryCreatorAgent",
            instructions: """
You are a creative storytelling agent specializing in LinkedIn content.
Based on the speaker information and session details provided, create an engaging story that:
- Highlights the speaker's expertise and background
- Emphasizes the value and uniqueness of the session
- Creates anticipation and excitement
- Uses storytelling techniques to capture attention
- Includes compelling hooks and insights

Write in a conversational, professional tone suitable for LinkedIn.
Length: 150-200 words.
""");

        // 4. Agent éditeur - Restructure et polit le contenu
        AIAgent editorAgent = new ChatClientAgent(_chatClient,
            name: "EditorAgent",
            instructions: """
You are a professional editor specializing in LinkedIn content.
Your task is to refine and polish the story to make it publication-ready:
- Improve clarity and flow
- Strengthen the narrative structure
- Optimize for LinkedIn engagement (use line breaks, emojis strategically)
- Add a compelling hook at the beginning
- End with a call-to-action
- Ensure professional yet engaging tone

Format with proper LinkedIn structure (short paragraphs, clear sections).
""");

        // 5. Agent traducteur bilingue - Produit versions FR et EN
        AIAgent translatorAgent = new ChatClientAgent(_chatClient,
            name: "TranslatorAgent",
            instructions: """
You are a bilingual translator specializing in professional content.
Take the polished LinkedIn post and create TWO versions:

1. **Version Française** - Natural, engaging French
2. **English Version** - Natural, engaging English

CRITICAL: You MUST return ONLY a valid JSON object with this EXACT format (no markdown, no code blocks, no additional text):

{
  "french": "Complete French version here...",
  "english": "Complete English version here..."
}

RULES:
- Return ONLY the JSON object, nothing else
- Do NOT wrap in ```json or ``` code blocks
- Do NOT add any explanatory text before or after
- Maintain the same tone, energy, and formatting in both languages
- Adapt cultural references appropriately for each audience
- Preserve emojis and formatting (line breaks, bold, etc.)
- Each version should be complete and ready to publish

Example of CORRECT output:
{
  "french": "🚀 Découvrez...",
  "english": "🚀 Discover..."
}

Example of WRONG output (DO NOT DO THIS):
```json
{
  "french": "...",
  "english": "..."
}
```
""");

        // Créer les executors personnalisés avec logger
        var mcpSearchExecutor = new McpSearchExecutor(mcpSearchAgent, _logger);
        var bingSearchExecutor = new BingSearchExecutor(bingSearchAgent, _logger);
        var storyCreatorExecutor = new StoryCreatorExecutor(storyCreatorAgent, _logger);
        var editorExecutor = new EditorExecutor(editorAgent, _logger);
        var translatorExecutor = new TranslatorExecutor(translatorAgent, _logger);

        // Construire le workflow séquentiel
        var builder = new WorkflowBuilder(mcpSearchExecutor);
        
        builder
            // MCP Search -> Bing Search (conditional: only if speaker found)
            .AddEdge<McpSearchResult>(mcpSearchExecutor, bingSearchExecutor, 
                condition: result => result?.Found ?? false)
            
            // Bing Search -> Story Creator
            .AddEdge(bingSearchExecutor, storyCreatorExecutor)
            
            // Story Creator -> Editor
            .AddEdge(storyCreatorExecutor, editorExecutor)
            
            // Editor -> Translator
            .AddEdge(editorExecutor, translatorExecutor)
            
            // Define workflow output
            .WithOutputFrom(translatorExecutor);

        _logger.LogInformation("LinkedIn Content Workflow built successfully with {ExecutorCount} executors", 5);
        
        return builder.Build();
    }
}

/// <summary>
/// Résultat de la recherche MCP
/// </summary>
public class McpSearchResult
{
    public bool Found { get; set; }
    public string? SpeakerName { get; set; }
    public string? SessionTitle { get; set; }
    public string? SessionDescription { get; set; }
    public string? SpeakerBio { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Résultat de la recherche Bing
/// </summary>
public class BingSearchResult
{
    public string? SpeakerName { get; set; }
    public string? AdditionalInfo { get; set; }
    public McpSearchResult? OriginalData { get; set; }
}

/// <summary>
/// Histoire créée
/// </summary>
public class StoryResult
{
    public string? Story { get; set; }
    public BingSearchResult? SourceData { get; set; }
}

/// <summary>
/// Contenu édité
/// </summary>
public class EditedContent
{
    public string? PolishedContent { get; set; }
}

/// <summary>
/// Résultat final bilingue
/// </summary>
public class BilingualContent
{
    public string? French { get; set; }
    public string? English { get; set; }
}

/// <summary>
/// Executor pour la recherche MCP
/// </summary>
public class McpSearchExecutor : Executor<ChatMessage, McpSearchResult>
{
    private readonly AIAgent _agent;
    private readonly ILogger _logger;

    public McpSearchExecutor(AIAgent agent, ILogger logger) : base("McpSearchExecutor")
    {
        _agent = agent;
        _logger = logger;
    }

    public override async ValueTask<McpSearchResult> HandleAsync(
        ChatMessage message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting MCP search for query: {Query}", message.Text);
        var startTime = DateTime.UtcNow;
        
        string? responseText = null;
        
        try
        {
            var response = await _agent.RunAsync([message], cancellationToken: cancellationToken);
            responseText = response.Text ?? "{}";
            
            _logger.LogDebug("MCP search raw response: {Response}", responseText);
            
            // Nettoyer le JSON si nécessaire (enlever les markdown code blocks)
            if (responseText.Contains("```json") || responseText.Contains("```"))
            {
                _logger.LogWarning("MCP search response contains markdown code blocks, cleaning up");
                var startIndex = responseText.IndexOf("{");
                var endIndex = responseText.LastIndexOf("}");
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    responseText = responseText.Substring(startIndex, endIndex - startIndex + 1);
                    _logger.LogDebug("Cleaned JSON: {CleanedJson}", responseText);
                }
            }
            
            var result = JsonSerializer.Deserialize<McpSearchResult>(responseText, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Found == false)
            {
                _logger.LogWarning("MCP search found no results for query: {Query}. Message: {Message}", 
                    message.Text, result.Message);
                await context.YieldOutputAsync(result.Message ?? "Aucune information trouvée pour cette recherche.");
            }
            else if (result != null)
            {
                _logger.LogInformation("MCP search successful. Found speaker: {SpeakerName}, Session: {SessionTitle}", 
                    result.SpeakerName, result.SessionTitle);
            }

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("MCP search completed in {Duration}ms", duration.TotalMilliseconds);

            return result ?? new McpSearchResult { Found = false, Message = "Erreur lors de la recherche" };
        }
        catch (JsonException ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "MCP search JSON parsing failed after {Duration}ms for query: {Query}. Raw response: {Response}", 
                duration.TotalMilliseconds, message.Text, responseText ?? "null");
            
            return new McpSearchResult 
            { 
                Found = false, 
                Message = $"Erreur de parsing JSON: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "MCP search failed after {Duration}ms for query: {Query}", 
                duration.TotalMilliseconds, message.Text);
            
            return new McpSearchResult 
            { 
                Found = false, 
                Message = $"Erreur lors de la recherche: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Executor pour la recherche Bing
/// </summary>
public class BingSearchExecutor : Executor<McpSearchResult, BingSearchResult>
{
    private readonly AIAgent _agent;
    private readonly ILogger _logger;

    public BingSearchExecutor(AIAgent agent, ILogger logger) : base("BingSearchExecutor")
    {
        _agent = agent;
        _logger = logger;
    }

    public override async ValueTask<BingSearchResult> HandleAsync(
        McpSearchResult message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Bing search for speaker: {SpeakerName}", message.SpeakerName);
        var startTime = DateTime.UtcNow;
        
        try
        {
            var searchQuery = $"Find information about {message.SpeakerName}, including their professional background, achievements, and contributions.";
            
            var response = await _agent.RunAsync(searchQuery, cancellationToken: cancellationToken);
            
            var isSkipped = response.Text?.Contains("SKIP_SEARCH") ?? false;
            if (isSkipped)
            {
                _logger.LogInformation("Bing search skipped by user request for speaker: {SpeakerName}", message.SpeakerName);
            }
            else
            {
                _logger.LogInformation("Bing search completed for speaker: {SpeakerName}. Response length: {Length} chars", 
                    message.SpeakerName, response.Text?.Length ?? 0);
            }
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Bing search completed in {Duration}ms", duration.TotalMilliseconds);
            
            return new BingSearchResult
            {
                SpeakerName = message.SpeakerName,
                AdditionalInfo = response.Text,
                OriginalData = message
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "Bing search failed after {Duration}ms for speaker: {SpeakerName}", 
                duration.TotalMilliseconds, message.SpeakerName);
            throw;
        }
    }
}

/// <summary>
/// Executor pour la création de story
/// </summary>
public class StoryCreatorExecutor : Executor<BingSearchResult, StoryResult>
{
    private readonly AIAgent _agent;
    private readonly ILogger _logger;

    public StoryCreatorExecutor(AIAgent agent, ILogger logger) : base("StoryCreatorExecutor")
    {
        _agent = agent;
        _logger = logger;
    }

    public override async ValueTask<StoryResult> HandleAsync(
        BingSearchResult message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating LinkedIn story for speaker: {SpeakerName}", message.SpeakerName);
        var startTime = DateTime.UtcNow;
        
        try
        {
            var prompt = $"""
Create an engaging LinkedIn story using this information:

Speaker: {message.OriginalData?.SpeakerName}
Session: {message.OriginalData?.SessionTitle}
Description: {message.OriginalData?.SessionDescription}
Speaker Bio: {message.OriginalData?.SpeakerBio}

Additional Research:
{message.AdditionalInfo}
""";
            
            _logger.LogDebug("Story creation prompt prepared. Prompt length: {Length} chars", prompt.Length);
            
            var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Story created successfully. Story length: {Length} chars", response.Text?.Length ?? 0);
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Story creation completed in {Duration}ms", duration.TotalMilliseconds);
            
            return new StoryResult
            {
                Story = response.Text,
                SourceData = message
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "Story creation failed after {Duration}ms for speaker: {SpeakerName}", 
                duration.TotalMilliseconds, message.SpeakerName);
            throw;
        }
    }
}

/// <summary>
/// Executor pour l'édition
/// </summary>
public class EditorExecutor : Executor<StoryResult, EditedContent>
{
    private readonly AIAgent _agent;
    private readonly ILogger _logger;

    public EditorExecutor(AIAgent agent, ILogger logger) : base("EditorExecutor")
    {
        _agent = agent;
        _logger = logger;
    }

    public override async ValueTask<EditedContent> HandleAsync(
        StoryResult message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting content editing. Input length: {Length} chars", message.Story?.Length ?? 0);
        var startTime = DateTime.UtcNow;
        
        try
        {
            var response = await _agent.RunAsync(message.Story ?? "", cancellationToken: cancellationToken);
            
            _logger.LogInformation("Content edited successfully. Output length: {Length} chars", response.Text?.Length ?? 0);
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Content editing completed in {Duration}ms", duration.TotalMilliseconds);
            
            return new EditedContent
            {
                PolishedContent = response.Text
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "Content editing failed after {Duration}ms", duration.TotalMilliseconds);
            throw;
        }
    }
}

/// <summary>
/// Executor pour la traduction bilingue
/// </summary>
public class TranslatorExecutor : Executor<EditedContent, BilingualContent>
{
    private readonly AIAgent _agent;
    private readonly ILogger _logger;

    public TranslatorExecutor(AIAgent agent, ILogger logger) : base("TranslatorExecutor")
    {
        _agent = agent;
        _logger = logger;
    }

    public override async ValueTask<BilingualContent> HandleAsync(
        EditedContent message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting bilingual translation. Input length: {Length} chars", message.PolishedContent?.Length ?? 0);
        var startTime = DateTime.UtcNow;
        
        string? responseText = null;
        
        try
        {
            var response = await _agent.RunAsync(message.PolishedContent ?? "", cancellationToken: cancellationToken);
            responseText = response.Text;
            
            _logger.LogDebug("Translation response received: {Response}", responseText);
            
            // Essayer de parser le JSON retourné par l'agent
            var jsonText = responseText ?? "{}";
            
            // Nettoyer le texte si nécessaire (enlever les markdown code blocks)
            if (jsonText.Contains("```json"))
            {
                _logger.LogWarning("Translation response contains markdown code blocks, cleaning up");
                var startIndex = jsonText.IndexOf("{");
                var endIndex = jsonText.LastIndexOf("}");
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    jsonText = jsonText.Substring(startIndex, endIndex - startIndex + 1);
                    _logger.LogDebug("Cleaned JSON: {CleanedJson}", jsonText);
                }
            }
            
            var result = JsonSerializer.Deserialize<BilingualContent>(jsonText, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (result != null && !string.IsNullOrEmpty(result.French) && !string.IsNullOrEmpty(result.English))
            {
                _logger.LogInformation("Translation successful. FR: {FrenchLength} chars, EN: {EnglishLength} chars", 
                    result.French.Length, result.English.Length);
                
                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation("Bilingual translation completed in {Duration}ms", duration.TotalMilliseconds);
                
                return result;
            }
            
            _logger.LogWarning("Translation parsing incomplete, using fallback");
            
            // Si la désérialisation échoue ou est incomplète, retourner le texte brut dans les deux langues
            return new BilingualContent 
            { 
                French = responseText ?? "Erreur de traduction", 
                English = responseText ?? "Translation error" 
            };
        }
        catch (JsonException ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "Translation JSON parsing failed after {Duration}ms. Raw response: {Response}", 
                duration.TotalMilliseconds, responseText ?? "null");
            
            await context.AddEventAsync(new WorkflowWarningEvent($"Translation parsing error: {ex.Message}"));
            
            // Retourner le texte brut comme fallback
            return new BilingualContent 
            { 
                French = responseText ?? "Erreur de traduction", 
                English = responseText ?? "Translation error" 
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "Translation failed after {Duration}ms", duration.TotalMilliseconds);
            throw;
        }
    }
}
