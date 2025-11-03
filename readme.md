# Plan, Do, Check… Agent !

> **Code avec honneur, debug avec courage** 🥋

Session pour le **DevDay 2025 - Bushido Code**  
📍 Mons, Belgium  
📅 Thursday, 13 Nov 2025  
🕙 10:30 am - 11:15 am (45 minutes)

---

## 📋 Informations de la session

| Propriété | Détail |
|-----------|--------|
| **Format** | Session |
| **Track** | AI |
| **Niveau** | Intermédiaire |
| **Langue** | Français |
| **Durée** | 45 minutes |

---

## 🎯 Description

Dans cette session, on décline le cycle **Plan-Do-Check-Act… en Plan-Do-Check-Agent**.

On ouvre le capot du **Microsoft Agent Framework** côté .NET : 
- 🧠 **Plan** : Planner & raisonnement
- 🛠️ **Do** : Tools (actions)
- 💾 **Check** : Mémoire/état
- 🎭 **Agent** : Orchestration (handoff, concurrent, group chat)

Démo à l'appui, vous repartez avec :
- Un squelette d'agent prêt à étendre
- Des patterns pragmatiques
- Les pièges à éviter

**Discipline du plan, honneur du résultat.**

---

## 🎤 Speaker

**Adrien Clerbois**  
Microsoft MVP

- 📧 [adrien.social](https://adrien.social)
- 🐙 [github.com/aclerbois](https://github.com/aclerbois)
- 💼 [linkedin.com/in/aclerbois](https://linkedin.com/in/aclerbois)

---

## 📚 Contenu du repository

### Slides

Les slides de la présentation sont disponibles dans le fichier [`slides/maf_slidedeck.html`](slides/maf_slidedeck.html).

> ⚠️ **Note** : Les slides sont actuellement en construction

Pour visualiser les slides :
1. Ouvrir le fichier `maf_slidedeck.html` dans un navigateur
2. Utiliser les flèches ← → ou les boutons de navigation
3. Utiliser les touches fléchées du clavier pour naviguer

### Démos

Les exemples de code sont disponibles dans le dossier [`src/`](src/).

#### 📁 Structure des démos

```
src/
├── 01-hello-world/                      🧠 PLAN - Premier agent simple
├── 02-vision-llm/                       🧠 PLAN - Agent multimodal (vision)
├── 03-multi-turn-agent/                 💾 CHECK - Conversation avec mémoire (thread)
├── 04-use-tool/                         🛠️ DO - Agent avec outils (functions)
└── 05-use-tool-with-human-interaction/  🎭 AGENT - Approbation humaine
```

---

#### 01-hello-world 🧠

**Pilier** : PLAN - Le Raisonnement

**Description** : Premier agent simple utilisant Azure OpenAI et Microsoft Agent Framework

**Ce que vous apprenez** :
- Connexion à Azure OpenAI avec `AzureCliCredential`
- Création d'un agent simple avec instructions personnalisées
- Exécution d'une requête basique

**Code clé** :
```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri("https://devday-2025-maf.openai.azure.com/"),
    new AzureCliCredential())
        .GetChatClient("mistral-medium-2505")
        .CreateAIAgent(
            instructions: "Tu es doué pour raconter des blagues sarcastiques.", 
            name: "Joker");

Console.WriteLine(await agent.RunAsync("Raconte-moi une blague sur un pirate."));
```

**Concepts** : Agent basique, Instructions, Single-turn conversation

---

#### 02-vision-llm 🧠

**Pilier** : PLAN - Raisonnement Multimodal

**Description** : Agent capable d'analyser des images en plus du texte

**Ce que vous apprenez** :
- Utilisation de modèles multimodaux (GPT-4o)
- Combinaison de texte et d'images dans un message
- Types de contenu : `TextContent` et `UriContent`

**Code clé** :
```csharp
AIAgent agent = new AzureOpenAIClient(...)
    .GetChatClient("gpt-4o")
    .CreateAIAgent(
        name: "VisionAgent",
        instructions: "Vous êtes un agent utile capable d'analyser des images.");

ChatMessage message = new(ChatRole.User, [
    new TextContent("Que voyez-vous dans cette image ?"),
    new UriContent("https://devday.be/assets/gallery-12.jpg", "image/jpeg")
]);

Console.WriteLine(await agent.RunAsync(message));
```

**Concepts** : Multimodal LLM, ChatMessage, ChatRole (User/Assistant/System/Tool)

---

#### 03-multi-turn-agent 💾

**Pilier** : CHECK - Mémoire Court Terme

**Description** : Agent capable de maintenir le contexte d'une conversation sur plusieurs tours

**Ce que vous apprenez** :
- Utilisation d'`AgentThread` pour maintenir le contexte
- Conversations multi-tours
- Références aux messages précédents

**Code clé** :
```csharp
AIAgent agent = new AzureOpenAIClient(...)
    .GetChatClient("gpt-4o")
    .CreateAIAgent(
        instructions: "Tu es doué pour raconter des blagues sarcastiques.", 
        name: "Joker");

AgentThread thread = agent.GetNewThread();

// Premier message
Console.WriteLine(await agent.RunAsync(
    "Raconte une blague au sujet des pirates.", thread));

// Deuxième message - l'agent se souvient de la blague précédente
Console.WriteLine(await agent.RunAsync(
    "Maintenant, ajoute des émojis et raconte-la avec la voix d'un perroquet.", 
    thread));
```

**Concepts** : AgentThread, Mémoire conversationnelle, Contexte persistant

---

#### 04-use-tool 🛠️

**Pilier** : DO - Actions avec Tools

**Description** : Agent capable d'utiliser des outils (functions) pour accéder à des données externes

**Ce que vous apprenez** :
- Définition de fonctions avec attributs `[Description]`
- Création d'outils avec `AIFunctionFactory`
- Function calling automatique par l'agent

**Code clé** :
```csharp
// Définition de la fonction outil
public class SpeakerTools
{
    [Description("Gets speaker information by last name.")]
    public static SpeakerInfo GetSpeakerByName(
        [Description("The last name of the speaker to retrieve.")] 
        string speakerLastName)
    {
        // Recherche dans la base de données des speakers
        return speakers.First(s => s.LastName == speakerLastName);
    }
}

// Création de l'agent avec l'outil
AIAgent agent = new AzureOpenAIClient(...)
    .GetChatClient("gpt-4o")
    .CreateAIAgent(
        instructions: "Tu es un assistant utile qui fournit des informations sur les intervenants de DevDay 2025.",
        tools: [AIFunctionFactory.Create(SpeakerTools.GetSpeakerByName)]);

Console.WriteLine(await agent.RunAsync(
    "Quelle session Adrien Clerbois présente-t-il ?"));
```

**Concepts** : Tools/Functions, Function Calling, Descriptions pour le LLM, `AIFunctionFactory`

---

#### 05-use-tool-with-human-interaction 🎭

**Pilier** : AGENT - Orchestration et Contrôle

**Description** : Agent qui demande une approbation humaine avant d'exécuter certaines fonctions sensibles

**Ce que vous apprenez** :
- Wrapping d'outils avec `ApprovalRequiredAIFunction`
- Interception des demandes d'exécution
- Workflow humain-dans-la-boucle (human-in-the-loop)
- Gestion des réponses d'approbation

**Code clé** :
```csharp
// Créer une fonction qui nécessite une approbation
AIFunction getSpeakerFunction = AIFunctionFactory.Create(SpeakerTools.GetSpeakerByName);
AIFunction approvalRequiredFunction = new ApprovalRequiredAIFunction(getSpeakerFunction);

AIAgent agent = new AzureOpenAIClient(...)
    .GetChatClient("gpt-4o")
    .CreateAIAgent(
        instructions: "Tu es un assistant utile...",
        tools: [approvalRequiredFunction]);

// Exécuter et intercepter les demandes d'approbation
AgentThread thread = agent.GetNewThread();
AgentRunResponse response = await agent.RunAsync(
    "Quelle session Adrien Clerbois présente-t-il ?", thread);

var functionApprovalRequests = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<FunctionApprovalRequestContent>()
    .ToList();

// Demander l'approbation à l'utilisateur
FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
Console.WriteLine($"Approbation requise pour '{requestContent.FunctionCall.Name}'");

// Approuver et continuer
var approvalMessage = new ChatMessage(ChatRole.User, 
    [requestContent.CreateResponse(true)]);
Console.WriteLine(await agent.RunAsync(approvalMessage, thread));
```

**Concepts** : Human-in-the-loop, Guardrails, `FunctionApprovalRequestContent`, Sécurité

---

### 🎯 Parcours d'apprentissage recommandé

1. **01-hello-world** → Comprendre les bases
2. **02-vision-llm** → Explorer les capacités multimodales
3. **03-multi-turn-agent** → Gérer la mémoire conversationnelle
4. **04-use-tool** → Connecter l'agent au monde réel
5. **05-use-tool-with-human-interaction** → Ajouter des guardrails

### 📦 Packages requis (communs à toutes les démos)

```xml
<PackageReference Include="Azure.AI.OpenAI" Version="2.5.0-beta.1" />
<PackageReference Include="Azure.Identity" Version="1.17.0" />
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-preview.251028.1" />
```

---

## 🔧 Configuration Azure AI Foundry

### Prérequis

- Un compte Azure actif
- Azure CLI installé ([Installation](https://learn.microsoft.com/cli/azure/install-azure-cli))
- .NET 10.0 SDK installé

### Étape 1 : Authentification Azure

```bash
# Se connecter à Azure
azd auth login

# Ou utiliser Azure CLI
az login

# Sélectionner votre subscription
az account set --subscription "VOTRE_SUBSCRIPTION_ID"

# Vérifier la subscription active
az account show
```

### Étape 2 : Créer une instance Azure AI Foundry

1. **Via le portail Azure** ([ai.azure.com](https://ai.azure.com))
   - Se connecter à Azure AI Foundry
   - Cliquer sur **"Create new project"**
   - Renseigner :
     - **Project name** : `devday-2025-maf` (ou votre nom)
     - **Subscription** : Sélectionner votre subscription
     - **Resource group** : Créer ou sélectionner un groupe (ex: `rg-devday-maf`)
     - **Location** : Choisir `West Europe` ou `France Central`
   - Cliquer sur **"Create"**

2. **Via Azure CLI** (alternative)
   ```bash
   # Créer un groupe de ressources
   az group create --name rg-devday-maf --location westeurope
   
   # Créer un Azure AI hub
   az ml workspace create \
     --kind hub \
     --resource-group rg-devday-maf \
     --name devday-2025-maf-hub
   
   # Créer un projet
   az ml workspace create \
     --kind project \
     --resource-group rg-devday-maf \
     --name devday-2025-maf \
     --hub-id /subscriptions/{subscription-id}/resourceGroups/rg-devday-maf/providers/Microsoft.MachineLearningServices/workspaces/devday-2025-maf-hub
   ```

### Étape 3 : Déployer un modèle

1. **Via Azure AI Foundry Portal**
   - Aller dans votre projet
   - Cliquer sur **"Deployments"** dans le menu de gauche
   - Cliquer sur **"+ Create deployment"**
   - Sélectionner un modèle :
     - **GPT-4o** : Modèle multimodal puissant
     - **GPT-4o mini** : Version légère et rapide
     - **Mistral Medium** : Alternative open source
   - Renseigner :
     - **Deployment name** : `mistral-medium-2505` (ou autre)
     - **Model version** : Dernière version disponible
     - **Tokens per Minute Rate Limit** : `10000` (ou selon besoin)
   - Cliquer sur **"Deploy"**

2. **Via Azure CLI** (alternative)
   ```bash
   az cognitiveservices account deployment create \
     --resource-group rg-devday-maf \
     --name devday-2025-maf \
     --deployment-name mistral-medium-2505 \
     --model-name mistral-medium \
     --model-version "2505" \
     --model-format OpenAI \
     --sku-capacity 10 \
     --sku-name "Standard"
   ```

### Étape 4 : Récupérer les informations de connexion

1. Dans Azure AI Foundry, aller dans **"Settings"** > **"Properties"**
2. Noter :
   - **Endpoint** : `https://VOTRE-RESOURCE.openai.azure.com/`
   - **Deployment name** : Le nom donné au modèle déployé
3. Mettre à jour votre code avec ces valeurs

### Étape 5 : Configurer l'authentification locale

Pour utiliser `AzureCliCredential` (comme dans les démos) :

```bash
# Se connecter avec Azure CLI
az login

# Configurer les permissions (si nécessaire)
az role assignment create \
  --assignee YOUR_USER_EMAIL \
  --role "Cognitive Services User" \
  --scope /subscriptions/{subscription-id}/resourceGroups/rg-devday-maf/providers/Microsoft.CognitiveServices/accounts/devday-2025-maf
```

### Étape 6 : Tester votre configuration

```bash
# Dans le dossier src/01-hello-world/
cd src/01-hello-world

# Restaurer les packages
dotnet restore

# Exécuter l'application
dotnet run
```

### 💡 Conseils

- **Coûts** : Commencez avec GPT-4o mini pour minimiser les coûts pendant le développement
- **Limites** : Configurez des quotas pour éviter les surprises
- **Monitoring** : Activez Application Insights pour suivre l'utilisation
- **Sécurité** : En production, utilisez Managed Identity au lieu d'Azure CLI Credential

### 🔗 Ressources utiles

- [Documentation Azure AI Foundry](https://learn.microsoft.com/azure/ai-studio/)
- [Déployer des modèles](https://learn.microsoft.com/azure/ai-studio/how-to/deploy-models)
- [Gérer les quotas](https://learn.microsoft.com/azure/ai-services/openai/how-to/quota)
- [Tarification Azure OpenAI](https://azure.microsoft.com/pricing/details/cognitive-services/openai-service/)

---

## 🏗️ Les 4 Piliers du Microsoft Agent Framework

### 🧠 PLAN - Le Raisonnement
- **Chat Completions** + Instructions
- Stratégies : ReAct, Chain-of-Thought, Function Calling
- Le cerveau qui raisonne

### 🛠️ DO - Les Actions
- **Tools/Functions** + Model Context Protocol (MCP)
- Intégration : Décorateurs .NET, MCP, OpenAPI
- Les mains qui agissent

### 💾 CHECK - La Mémoire
- **AgentThread** (mémoire court terme) + **Memory** (mémoire long terme)
- Context Providers
- La mémoire qui apprend

### 🎭 AGENT - L'Orchestration
- **Workflows** + Orchestration
- Patterns : Handoff, Concurrent, Conditional Routing
- La coordination qui optimise

---

## ⚠️ Les 4 Pièges à Éviter

1. **Plan sans discipline** : Trop de liberté = hallucinations + coûts
   - ✅ Solution : Guardrails, contraintes, budget tokens

2. **Tools mal documentés** : Descriptions floues = mauvais choix
   - ✅ Solution : Descriptions détaillées, tests isolés

3. **Mémoire non gérée** : Explosion du contexte = perte infos
   - ✅ Solution : AgentThread + Memory ensemble

4. **Sur-orchestration** : Trop d'agents = complexité ingérable
   - ✅ Solution : Commencer simple, diviser si justifié

---

## 🚀 Pour Démarrer

### Installation

```bash
dotnet add package Microsoft.SemanticKernel
```

### Ressources

- **Microsoft Agent Framework** : [docs.microsoft.com/azure/ai](https://docs.microsoft.com/azure/ai)
- **GitHub Semantic Kernel** : [github.com/microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel)
- **Azure AI Foundry** : [ai.azure.com](https://ai.azure.com)

### Communauté

- Discord Semantic Kernel : Discussions actives, Q&A
- Microsoft Learn : Modules de formation gratuits
- Samples GitHub : Exemples d'applications complètes

---

## 📝 Ce que vous allez apprendre

✅ **Concepts maîtrisés**
- Architecture d'un agent
- Les 4 piliers du MAF
- Patterns d'orchestration

✅ **Pratiques terrain**
- Pièges à éviter
- Best practices
- Code prêt à étendre

---

## 🎬 Démo

La démo présentera un **Support Client Multi-Agents** avec :
- **TriageAgent** : Analyse et catégorisation des demandes
- **TechAgent** : RAG sur documentation technique
- **BillingAgent** : Accès base de données
- Escalation automatique si confidence < 0.7
- Observabilité complète : traces, logs, métriques

**Technologies** : MAF + Azure OpenAI + Azure AI Search + Azure CosmosDB

---

## 🏛️ Workflow de Développement

1. **Commencer simple** : 1 agent, 2-3 tools, instructions claires
2. **Itérer progressivement** :
   - Ajouter Memory quand le contexte devient trop grand
   - Ajouter des tools selon les besoins
   - Diviser en multi-agents seulement si nécessaire
3. **Observer et mesurer** : Azure AI Foundry pour telemetry
   - Temps de réponse
   - Coût par interaction
   - Taux de succès
4. **Tester rigoureusement** : Validation des tools, edge cases, fallbacks

---

## 📜 Philosophie Bushidō Code

> **Discipline & Pragmatisme** : La tentation est d'ajouter features. La discipline est de rester simple jusqu'à ce que la complexité soit justifiée.

> **Honneur du résultat** : Un agent simple qui fonctionne > Un système complexe qui échoue

---

## 📅 Event

**DevDay 2025 - Bushido Code**  
13 novembre 2025  
Palais des Congrès, Mons, Belgium

---

## 📄 License

Ce contenu est fourni à des fins éducatives dans le cadre du DevDay 2025.

---

*武士道 - Bushidō Code*
