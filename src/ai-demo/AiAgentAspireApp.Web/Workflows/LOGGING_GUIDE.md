# 📊 Logging dans le Workflow LinkedIn

## Vue d'ensemble

Le workflow LinkedIn Content Creator intègre désormais un **logging structuré complet** avec `ILogger` pour une observabilité maximale dans **Aspire Dashboard**.

## 🎯 Points de Logging

### 1. **LinkedInContentWorkflow** (Workflow Principal)

```csharp
_logger.LogInformation("Building LinkedIn Content Workflow");
_logger.LogInformation("Loaded {ToolCount} MCP tools", mcpTools.Count());
_logger.LogInformation("LinkedIn Content Workflow built successfully with {ExecutorCount} executors", 5);
```

**Métriques logged :**
- Nombre d'outils MCP chargés
- Nombre d'executors dans le workflow

### 2. **McpSearchExecutor** (Recherche DevDay)

```csharp
_logger.LogInformation("Starting MCP search for query: {Query}", message.Text);
_logger.LogDebug("MCP search response: {Response}", resultText);
_logger.LogWarning("MCP search found no results for query: {Query}. Message: {Message}", message.Text, result.Message);
_logger.LogInformation("MCP search successful. Found speaker: {SpeakerName}, Session: {SessionTitle}", result?.SpeakerName, result?.SessionTitle);
_logger.LogInformation("MCP search completed in {Duration}ms", duration.TotalMilliseconds);
_logger.LogError(ex, "MCP search failed after {Duration}ms for query: {Query}", duration.TotalMilliseconds, message.Text);
```

**Métriques logged :**
- Requête utilisateur
- Durée d'exécution (ms)
- Speaker trouvé
- Titre de session
- Erreurs avec stack trace

### 3. **BingSearchExecutor** (Recherche Complémentaire)

```csharp
_logger.LogInformation("Starting Bing search for speaker: {SpeakerName}", message.SpeakerName);
_logger.LogInformation("Bing search skipped by user request for speaker: {SpeakerName}", message.SpeakerName);
_logger.LogInformation("Bing search completed for speaker: {SpeakerName}. Response length: {Length} chars", message.SpeakerName, response.Text?.Length ?? 0);
_logger.LogInformation("Bing search completed in {Duration}ms", duration.TotalMilliseconds);
_logger.LogError(ex, "Bing search failed after {Duration}ms for speaker: {SpeakerName}", duration.TotalMilliseconds, message.SpeakerName);
```

**Métriques logged :**
- Speaker recherché
- Skip status (si utilisateur a demandé)
- Longueur de la réponse (chars)
- Durée d'exécution (ms)
- Erreurs avec stack trace

### 4. **StoryCreatorExecutor** (Création d'Histoire)

```csharp
_logger.LogInformation("Creating LinkedIn story for speaker: {SpeakerName}", message.SpeakerName);
_logger.LogDebug("Story creation prompt prepared. Prompt length: {Length} chars", prompt.Length);
_logger.LogInformation("Story created successfully. Story length: {Length} chars", response.Text?.Length ?? 0);
_logger.LogInformation("Story creation completed in {Duration}ms", duration.TotalMilliseconds);
_logger.LogError(ex, "Story creation failed after {Duration}ms for speaker: {SpeakerName}", duration.TotalMilliseconds, message.SpeakerName);
```

**Métriques logged :**
- Speaker pour lequel l'histoire est créée
- Longueur du prompt (chars)
- Longueur de l'histoire (chars)
- Durée d'exécution (ms)
- Erreurs avec stack trace

### 5. **EditorExecutor** (Édition)

```csharp
_logger.LogInformation("Starting content editing. Input length: {Length} chars", message.Story?.Length ?? 0);
_logger.LogInformation("Content edited successfully. Output length: {Length} chars", response.Text?.Length ?? 0);
_logger.LogInformation("Content editing completed in {Duration}ms", duration.TotalMilliseconds);
_logger.LogError(ex, "Content editing failed after {Duration}ms", duration.TotalMilliseconds);
```

**Métriques logged :**
- Longueur d'entrée (chars)
- Longueur de sortie (chars)
- Durée d'exécution (ms)
- Erreurs avec stack trace

### 6. **TranslatorExecutor** (Traduction)

```csharp
_logger.LogInformation("Starting bilingual translation. Input length: {Length} chars", message.PolishedContent?.Length ?? 0);
_logger.LogDebug("Translation response received: {Response}", responseText);
_logger.LogWarning("Translation response contains markdown code blocks, cleaning up");
_logger.LogDebug("Cleaned JSON: {CleanedJson}", jsonText);
_logger.LogInformation("Translation successful. FR: {FrenchLength} chars, EN: {EnglishLength} chars", result.French.Length, result.English.Length);
_logger.LogInformation("Bilingual translation completed in {Duration}ms", duration.TotalMilliseconds);
_logger.LogWarning("Translation parsing incomplete, using fallback");
_logger.LogError(ex, "Translation JSON parsing failed after {Duration}ms. Raw response: {Response}", duration.TotalMilliseconds, responseText ?? "null");
_logger.LogError(ex, "Translation failed after {Duration}ms", duration.TotalMilliseconds);
```

**Métriques logged :**
- Longueur d'entrée (chars)
- Longueur version française (chars)
- Longueur version anglaise (chars)
- Durée d'exécution (ms)
- Warnings sur nettoyage JSON
- Erreurs de parsing JSON
- Erreurs avec stack trace

### 7. **AiChat** (UI Component)

```csharp
Logger.LogInformation("User submitted query: {Query}", userInput);
Logger.LogInformation("Building LinkedIn Content Workflow for query: {Query}", currentUserInput);
Logger.LogInformation("Starting workflow execution");
Logger.LogDebug("Workflow event #{Count}: {EventType}", eventCount, evt.GetType().Name);
Logger.LogInformation("Executor started: {ExecutorId} - {Stage}", invokedEvent.ExecutorId, currentStage);
Logger.LogInformation("Executor completed: {ExecutorId}", completedEvent.ExecutorId);
Logger.LogWarning("Workflow stopped: No content found for query '{Query}'", currentUserInput);
Logger.LogDebug("Agent update from {ExecutorId}: {DataLength} chars", updateEvent.ExecutorId, updateEvent.Data.ToString()?.Length ?? 0);
Logger.LogInformation("Workflow output received");
Logger.LogInformation("Successfully extracted bilingual content. FR: {FrLength} chars, EN: {EnLength} chars", bilingualContent.French?.Length ?? 0, bilingualContent.English?.Length ?? 0);
Logger.LogWarning("Failed to extract bilingual content, using raw output");
Logger.LogError("Workflow error occurred");
Logger.LogInformation("Workflow completed successfully in {Duration}ms. Total events: {EventCount}", workflowDuration.TotalMilliseconds, eventCount);
Logger.LogError(ex, "Workflow failed after {Duration}ms for query: {Query}", workflowDuration.TotalMilliseconds, currentUserInput);
Logger.LogDebug("Cleaning markdown code blocks from bilingual content");
Logger.LogError(ex, "JSON parsing error while extracting bilingual content");
Logger.LogError(ex, "Error extracting bilingual content");
```

**Métriques logged :**
- Requête utilisateur
- Durée totale du workflow (ms)
- Nombre d'événements traités
- Détails de chaque executor
- Longueurs de contenu à chaque étape
- Erreurs avec stack trace

## 📈 Niveaux de Log

| Niveau | Usage | Exemples |
|--------|-------|----------|
| **Debug** | Détails techniques | Réponses JSON, longueurs de prompts |
| **Information** | Flux normal | Début/fin d'executor, métriques |
| **Warning** | Situations non-idéales | Nettoyage JSON, parsing incomplet |
| **Error** | Erreurs avec recovery | Erreurs de parsing avec fallback |
| **Critical** | Erreurs fatales | (Non utilisé actuellement) |

## 🔍 Visualisation dans Aspire Dashboard

### Filtrage par Catégorie

**Workflow Général :**
```
Category: AiAgentAspireApp.Web.Workflows.LinkedInContentWorkflow
```

**MCP Search :**
```
Category: AiAgentAspireApp.Web.Workflows.McpSearchExecutor
```

**Bing Search :**
```
Category: AiAgentAspireApp.Web.Workflows.BingSearchExecutor
```

**Story Creator :**
```
Category: AiAgentAspireApp.Web.Workflows.StoryCreatorExecutor
```

**Editor :**
```
Category: AiAgentAspireApp.Web.Workflows.EditorExecutor
```

**Translator :**
```
Category: AiAgentAspireApp.Web.Workflows.TranslatorExecutor
```

**UI Component :**
```
Category: AiAgentAspireApp.Web.Components.Pages.AiChat
```

### Propriétés Structurées

Toutes les métriques sont loggées avec des **propriétés structurées** pour faciliter le filtrage et l'analyse :

```json
{
  "Query": "Scott Hanselman",
  "SpeakerName": "Scott Hanselman",
  "SessionTitle": "Building Modern Cloud Apps",
  "Duration": 1234.56,
  "FrenchLength": 450,
  "EnglishLength": 420,
  "EventCount": 12
}
```

## 📊 Requêtes d'Analyse Recommandées

### 1. Performance par Executor

**Kusto Query (Aspire Dashboard) :**
```kusto
traces
| where category startswith "AiAgentAspireApp.Web.Workflows"
| where message contains "completed in"
| extend Duration = todouble(customDimensions.Duration)
| summarize avg(Duration), max(Duration), min(Duration) by category
| order by avg_Duration desc
```

### 2. Taux d'Erreur

```kusto
traces
| where category startswith "AiAgentAspireApp.Web"
| summarize 
    Total = count(),
    Errors = countif(severityLevel >= 3),
    ErrorRate = (countif(severityLevel >= 3) * 100.0) / count()
by category
```

### 3. Requêtes Sans Résultats

```kusto
traces
| where category == "AiAgentAspireApp.Web.Workflows.McpSearchExecutor"
| where message contains "found no results"
| project timestamp, customDimensions.Query
```

### 4. Distribution de Longueur de Contenu

```kusto
traces
| where category == "AiAgentAspireApp.Web.Workflows.TranslatorExecutor"
| where message contains "Translation successful"
| extend FrenchLength = toint(customDimensions.FrenchLength)
| extend EnglishLength = toint(customDimensions.EnglishLength)
| summarize 
    avgFrench = avg(FrenchLength), 
    avgEnglish = avg(EnglishLength)
```

### 5. Timeline d'un Workflow Complet

```kusto
traces
| where category contains "AiAgentAspireApp.Web"
| where timestamp > ago(1h)
| project timestamp, category, message, customDimensions
| order by timestamp asc
```

## 🎯 Dashboards Recommandés

### Dashboard Performance

**Métriques clés :**
- Durée moyenne par executor
- Durée totale du workflow
- P95, P99 de chaque étape
- Tendance sur 24h

### Dashboard Qualité

**Métriques clés :**
- Taux de succès (MCP found)
- Taux d'erreur de parsing JSON
- Longueur moyenne du contenu généré
- Ratio French/English length

### Dashboard Utilisation

**Métriques clés :**
- Nombre de requêtes par heure
- Top speakers recherchés
- Distribution des longueurs de requêtes
- Taux de skip de Bing search

## 🔧 Configuration du Logging

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AiAgentAspireApp.Web.Workflows": "Debug",
      "AiAgentAspireApp.Web.Components.Pages": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Programme.cs (Aspire AppHost)

```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
});
```

## 🚀 Meilleures Pratiques

### 1. Logging Structuré

✅ **Bon :**
```csharp
_logger.LogInformation("MCP search completed in {Duration}ms for speaker: {SpeakerName}", 
    duration.TotalMilliseconds, speakerName);
```

❌ **Mauvais :**
```csharp
_logger.LogInformation($"MCP search completed in {duration.TotalMilliseconds}ms for speaker: {speakerName}");
```

### 2. Contexte dans les Erreurs

✅ **Bon :**
```csharp
_logger.LogError(ex, "Translation failed after {Duration}ms for query: {Query}", 
    duration.TotalMilliseconds, query);
```

❌ **Mauvais :**
```csharp
_logger.LogError(ex.Message);
```

### 3. Niveaux Appropriés

✅ **Bon :**
- `Debug`: Détails internes (réponses JSON, prompts)
- `Information`: Flow normal (début/fin d'executors)
- `Warning`: Situations récupérables (nettoyage JSON)
- `Error`: Erreurs nécessitant attention

### 4. Performance

- N'loggez les gros objets qu'en `Debug`
- Utilisez `{@Object}` pour serialisation JSON automatique
- Limitez le logging en production sensible

## 📖 Exemples de Logs Typiques

### Workflow Réussi

```
[14:30:01 INF] User submitted query: Scott Hanselman
[14:30:01 INF] Building LinkedIn Content Workflow for query: Scott Hanselman
[14:30:01 INF] Building LinkedIn Content Workflow
[14:30:02 INF] Loaded 3 MCP tools
[14:30:02 INF] LinkedIn Content Workflow built successfully with 5 executors
[14:30:02 INF] Starting workflow execution
[14:30:02 INF] Starting MCP search for query: Scott Hanselman
[14:30:03 INF] MCP search successful. Found speaker: Scott Hanselman, Session: Building Modern Cloud Apps
[14:30:03 INF] MCP search completed in 1234ms
[14:30:03 INF] Starting Bing search for speaker: Scott Hanselman
[14:30:05 INF] Bing search completed for speaker: Scott Hanselman. Response length: 450 chars
[14:30:05 INF] Bing search completed in 2100ms
[14:30:05 INF] Creating LinkedIn story for speaker: Scott Hanselman
[14:30:08 INF] Story created successfully. Story length: 650 chars
[14:30:08 INF] Story creation completed in 3200ms
[14:30:08 INF] Starting content editing. Input length: 650 chars
[14:30:10 INF] Content edited successfully. Output length: 720 chars
[14:30:10 INF] Content editing completed in 2300ms
[14:30:10 INF] Starting bilingual translation. Input length: 720 chars
[14:30:13 INF] Translation successful. FR: 680 chars, EN: 650 chars
[14:30:13 INF] Bilingual translation completed in 3100ms
[14:30:13 INF] Workflow output received
[14:30:13 INF] Successfully extracted bilingual content. FR: 680 chars, EN: 650 chars
[14:30:13 INF] Workflow completed successfully in 12234ms. Total events: 15
```

### Workflow avec Erreur de Parsing JSON

```
[14:35:10 INF] Starting bilingual translation. Input length: 720 chars
[14:35:10 DBG] Translation response received: ```json\n{\n  "french": "...",\n  "english": "..."\n}\n```
[14:35:10 WRN] Translation response contains markdown code blocks, cleaning up
[14:35:10 DBG] Cleaned JSON: {\n  "french": "...",\n  "english": "..."\n}
[14:35:10 INF] Translation successful. FR: 680 chars, EN: 650 chars
[14:35:13 INF] Bilingual translation completed in 3100ms
```

### Workflow Sans Résultats

```
[14:40:01 INF] User submitted query: John Doe
[14:40:02 INF] Starting MCP search for query: John Doe
[14:40:03 WRN] MCP search found no results for query: John Doe. Message: No speaker found with this name
[14:40:03 INF] MCP search completed in 1000ms
[14:40:03 WRN] Workflow stopped: No content found for query 'John Doe'
```

## 🎓 Références

- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [ASP.NET Core Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)
- [Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard)
- [Structured Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#log-message-template)

---

**Le workflow LinkedIn est maintenant complètement observable dans Aspire Dashboard ! 🎉**
