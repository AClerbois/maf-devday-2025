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