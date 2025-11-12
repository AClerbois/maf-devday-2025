# LinkedIn Content Creator Workflow

## Vue d'ensemble

Ce workflow utilise **Microsoft Agent Framework** pour automatiser la création de contenu LinkedIn professionnel bilingue (français et anglais) à partir des informations de speakers et sessions DevDay.

## Architecture du Workflow

Le workflow implémente un pattern **séquentiel** avec 5 agents spécialisés :

```
User Input → MCP Search → Bing Search → Story Creator → Editor → Translator → Bilingual Output
```

### Agents du Workflow

#### 1. **McpSearchAgent** 🔍
- **Rôle** : Recherche dans les données DevDay via MCP (Model Context Protocol)
- **Entrée** : Nom du speaker ou titre de session
- **Sortie** : `McpSearchResult` (JSON)
  - `found`: boolean
  - `speakerName`: string
  - `sessionTitle`: string
  - `sessionDescription`: string
  - `speakerBio`: string
  - `message`: string (si non trouvé)
- **Outils** : MCP Tools via McpClient
- **Condition** : Si `found = false`, le workflow s'arrête et informe l'utilisateur

#### 2. **BingSearchAgent** 🌐
- **Rôle** : Enrichissement d'informations sur le speaker
- **Entrée** : `McpSearchResult`
- **Sortie** : `BingSearchResult`
  - Informations professionnelles
  - Réalisations récentes
  - Présence sur les réseaux sociaux
  - Projets notables
- **Déclenchement** : Seulement si MCP Search a trouvé des résultats

#### 3. **StoryCreatorAgent** ✍️
- **Rôle** : Création d'une histoire LinkedIn engageante
- **Entrée** : `BingSearchResult` (avec données MCP et Bing)
- **Sortie** : `StoryResult`
- **Caractéristiques** :
  - Storytelling captivant
  - Mise en valeur de l'expertise du speaker
  - Emphase sur l'unicité de la session
  - Ton conversationnel et professionnel
  - Longueur : 150-200 mots

#### 4. **EditorAgent** 📝
- **Rôle** : Édition et polissage du contenu
- **Entrée** : `StoryResult`
- **Sortie** : `EditedContent`
- **Améliorations** :
  - Clarté et fluidité
  - Structure narrative renforcée
  - Optimisation pour l'engagement LinkedIn
  - Ajout de hooks accrocheurs
  - Call-to-action efficace
  - Formatage professionnel (paragraphes courts, emojis stratégiques)

#### 5. **TranslatorAgent** 🌍
- **Rôle** : Traduction bilingue (FR/EN)
- **Entrée** : `EditedContent`
- **Sortie** : `BilingualContent` (JSON)
  ```json
  {
    "french": "Version française naturelle...",
    "english": "Natural English version..."
  }
  ```
- **Caractéristiques** :
  - Traduction naturelle (pas littérale)
  - Adaptation culturelle appropriée
  - Maintien du ton et de l'énergie
  - Conservation du formatage

## Flux d'Exécution

### Étape 1 : Recherche MCP
```csharp
User: "Scott Hanselman"
↓
McpSearchExecutor → McpSearchAgent → MCP Tools
↓
{
  "found": true,
  "speakerName": "Scott Hanselman",
  "sessionTitle": "Building Modern Cloud Apps",
  "sessionDescription": "...",
  "speakerBio": "..."
}
```

**Si `found = false`** : Workflow s'arrête avec message utilisateur

### Étape 2 : Enrichissement Bing (Conditionnelle)
```csharp
McpSearchResult (found=true)
↓
BingSearchExecutor → BingSearchAgent
↓
{
  "speakerName": "Scott Hanselman",
  "additionalInfo": "Professional background, achievements...",
  "originalData": { ... }
}
```

### Étape 3 : Création de Story
```csharp
BingSearchResult
↓
StoryCreatorExecutor → StoryCreatorAgent
↓
{
  "story": "Engaging LinkedIn story (150-200 words)...",
  "sourceData": { ... }
}
```

### Étape 4 : Édition
```csharp
StoryResult
↓
EditorExecutor → EditorAgent
↓
{
  "polishedContent": "Polished, publication-ready content..."
}
```

### Étape 5 : Traduction Bilingue
```csharp
EditedContent
↓
TranslatorExecutor → TranslatorAgent
↓
{
  "french": "🎯 [Version FR optimisée]...",
  "english": "🎯 [Optimized EN version]..."
}
```

## Interface Utilisateur

### Indicateurs de Progression
L'interface affiche l'état du workflow en temps réel :

- 🔍 **Recherche dans les données DevDay**
- 🌐 **Recherche d'informations complémentaires sur le speaker**
- ✍️ **Création de l'histoire LinkedIn**
- 📝 **Édition et polissage du contenu**
- 🌍 **Traduction en français et anglais**
- ✅ **Contenu prêt à publier**

### Résultat Final
```markdown
# 🎯 Contenu LinkedIn Prêt à Publier

---

## 🇫🇷 Version Française

[Contenu optimisé en français]

---

## 🇬🇧 English Version

[Optimized English content]

---
```

## Gestion des Erreurs

### Scénarios d'Erreur

1. **Aucun résultat MCP** :
   - Détection : `McpSearchResult.Found = false`
   - Action : Arrêt du workflow
   - Message : "❌ Aucune information trouvée pour cette recherche."

2. **Erreur d'exécution** :
   - Détection : `WorkflowErrorEvent`
   - Action : Affichage d'un message générique
   - Message : "❌ Une erreur s'est produite lors du traitement du workflow."

3. **Exception inattendue** :
   - Détection : `catch (Exception ex)`
   - Action : Affichage du message d'erreur
   - Message : "❌ Erreur : {ex.Message}"

## Événements du Workflow

Le workflow émet plusieurs types d'événements :

| Événement | Description | Utilisation |
|-----------|-------------|-------------|
| `ExecutorInvokedEvent` | Un executor démarre | Mise à jour de l'UI avec le stage actuel |
| `ExecutorCompletedEvent` | Un executor termine | Vérification des résultats intermédiaires |
| `AgentRunUpdateEvent` | Mise à jour de streaming d'agent | Affichage en temps réel du contenu généré |
| `WorkflowOutputEvent` | Output final du workflow | Extraction et formatage du résultat bilingue |
| `WorkflowErrorEvent` | Erreur dans le workflow | Gestion d'erreur |

## Utilisation

### Exemples de Requêtes

**Par nom de speaker :**
```
Scott Hanselman
Satya Nadella
Mark Russinovich
```

**Par titre de session :**
```
Building Modern Cloud Apps
Introduction to Azure AI
DevOps Best Practices
```

### Workflow Complet

```csharp
// 1. Créer le workflow
var workflowBuilder = new LinkedInContentWorkflow(chatClient, mcpClient);
var workflow = await workflowBuilder.BuildWorkflowAsync();

// 2. Exécuter avec streaming
StreamingRun run = await InProcessExecution.StreamAsync(
    workflow, 
    new ChatMessage(ChatRole.User, "Scott Hanselman"));

await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

// 3. Traiter les événements
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    // Gérer les événements selon leur type
}
```

## Architecture Technique

### Technologies Utilisées

- **Microsoft.Agents.AI** : Framework d'agents IA
- **Microsoft.Agents.AI.Workflows** : Orchestration de workflows
- **Microsoft.Extensions.AI** : Abstractions IA
- **ModelContextProtocol** : Client MCP pour accès aux données DevDay
- **Blazor Server** : Interface utilisateur interactive
- **Markdig** : Rendu Markdown

### Patterns Implémentés

1. **Sequential Workflow Pattern** : Chaînage d'agents dans un ordre prédéfini
2. **Conditional Routing** : Routing basé sur des conditions (`found = true/false`)
3. **Streaming Execution** : Traitement en temps réel avec événements
4. **Type-Safe Executors** : Executors fortement typés pour chaque transformation

### Avantages de l'Architecture

✅ **Modularité** : Chaque agent a une responsabilité unique  
✅ **Réutilisabilité** : Les agents peuvent être utilisés dans d'autres workflows  
✅ **Testabilité** : Chaque executor peut être testé indépendamment  
✅ **Observabilité** : Suivi en temps réel via les événements  
✅ **Extensibilité** : Facile d'ajouter de nouveaux agents ou étapes  
✅ **Type Safety** : Validation au compile-time des types de messages  

## Améliorations Futures

### Possibles Extensions

1. **Cache de Résultats** : Éviter les recherches répétées
2. **Retry Logic** : Réessayer automatiquement en cas d'échec
3. **Checkpointing** : Sauvegarder l'état pour reprendre plus tard
4. **Multi-Language** : Supporter plus de langues (ES, DE, IT, etc.)
5. **Templates Personnalisables** : Permettre différents styles de posts
6. **Image Generation** : Générer des visuels accompagnant le post
7. **Scheduling** : Planifier la publication directement sur LinkedIn
8. **A/B Testing** : Générer plusieurs versions et choisir la meilleure

## Références

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [Workflow Orchestrations](https://learn.microsoft.com/en-us/agent-framework/user-guide/workflows/orchestrations/overview)
- [Sequential Orchestration](https://learn.microsoft.com/en-us/agent-framework/user-guide/workflows/orchestrations/sequential)
- [Model Context Protocol](https://modelcontextprotocol.io/)

---

**Créé avec ❤️ en utilisant Microsoft Agent Framework**
