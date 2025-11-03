using System.ComponentModel;
using System.Linq;

namespace Maf;

public class SpeakerTools
{
    [Description("Gets speaker information by last name.")]
    public static SpeakerInfo GetSpeakerByName(
        [Description("The last name of the speaker to retrieve.")] string speakerLastName)
    {
        foreach (var speaker in from speaker in Speakers
                                where string.Equals(speaker.LastName, speakerLastName, StringComparison.OrdinalIgnoreCase)
                                select speaker)
        {
            return speaker;
        }

        throw new ArgumentException($"Speaker with last name '{speakerLastName}' not found.");
    }

    private static readonly SpeakerInfo[] Speakers =
    [
        new SpeakerInfo("David", "Rousset", "IA + Devs : la nouvelle équation du code"),
        new SpeakerInfo("Alexandra", "Zakharova", "Keynote IA & Veille Technologique pour Étudiants"),
        new SpeakerInfo("Elaine", "Dias Batista", "AI Agents Face-Off: Same App, Multiple Frameworks"),
        new SpeakerInfo("Thierno", "Diallo", "🌱 Designing, Building and Optimizing APIs with Api Green Score Framework"),
        new SpeakerInfo("Simon", "Baudart", "Machine Learning comme un rōnin : zéro cloud, 100% contrôle"),
        new SpeakerInfo("Eric", "Decossaux", "Et toi ? Tu codes comme un samouraï ou comme un ronin ?"),
        new SpeakerInfo("Denis", "Voituron", "FluentUI Blazor: Le combo gagnant pour des applis qui ont du style"),
        new SpeakerInfo("Peter", "Eijgermans", "API Alchemy: Transforming Enterprise Endpoints for the AI Agent Revolution"),
        new SpeakerInfo("Sebastian", "Nilsson", "Next.js: Build a State-of-the-Art E-commerce in Fullstack React"),
        new SpeakerInfo("Anaïs", "Moulin", "Un sprint à Tokyo : voyage Agile au Japon"),
        new SpeakerInfo("Sébastien", "Warin", "Construire son propre processeur : du silicium au code"),
        new SpeakerInfo("Adrien", "Clerbois", "Plan, Do, Check… Agent ! — Construire des agents avec Microsoft Agent Framework"),
        new SpeakerInfo("Bernard", "Ludovic", "Le moral comme KPI"),
        new SpeakerInfo("Philippe", "Vlérick", "Comprendre les compilateurs : plus simple qu’il n’y paraît !"),
        new SpeakerInfo("Maximilien", "Charlier", "Intégration de capteurs IoT avec AWS Cloud : mesurez tout, à moindre coût"),
        new SpeakerInfo("Olivier", "Breda", "Failure MUST be an option"),
        new SpeakerInfo("Come", "Redon", "The art of Mona, from building to deploying with Ninjas"),
        new SpeakerInfo("Christophe", "Peugnet", "Blazor & .NET 10 – Plus rapide, plus clair, plus fiable"),
        new SpeakerInfo("Emmanuelle", "Hemmer", "IA et biais sexistes : comprendre, mesurer, corriger"),
        new SpeakerInfo("Stefan", "Fercot", "The art of data retention in PostgreSQL"),
        new SpeakerInfo("Christophe", "Gigax", "Optimize your event-driven architectures with Drasi"),
        new SpeakerInfo("Niels", "Tanis", "Using GenAI on and inside your code, what could possibly go wrong?"),
        new SpeakerInfo("Gilles", "Flisch", ".NET Aspire et architecture micro-service à l'aide de YARP comme reverse proxy."),
        new SpeakerInfo("Mitsuru", "Furuta", "AI4Fun"),
        new SpeakerInfo("David", "Rousset", ""),
        new SpeakerInfo("Antoine", "Smet", "AI Act : l’Europe trace la voie d’une IA de confiance"),
        new SpeakerInfo("Dieter", "Gobeyn", "AI Meets Integration: Building Smart Agents in Azure Logic Apps"),
    ];
}