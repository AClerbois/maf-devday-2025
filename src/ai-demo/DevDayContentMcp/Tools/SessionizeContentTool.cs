using System.ComponentModel;
using DevDayContentMcp.Services;
using ModelContextProtocol.Server;

namespace DevDayContentMcp.Tools;

[McpServerToolType]
public class SessionizeContentTool(ISessionizeService sessionizeService)
{
    [McpServerTool, Description("Retrieve session from the devday database with a given speaker name")]
    public async Task<IList<string>> RetrieveTheSessionOfTheDevDay(
        [Description("Description the first name")] string firstname,
        [Description("Description the last name")] string lastname)
    {
        var idsSpeaker = sessionizeService.GetSpeakers()
            .Where(c =>
                string.Compare(c.firstName, firstname, true) == 0
                && string.Compare(c.lastName, lastname, true) == 0)
            .Select(c => c.id)
            .ToList();

        return [.. sessionizeService.GetSessions()
            .Where(c => c.speakers.Any(d => idsSpeaker.Contains(d)))
            .Select(c => $"Title: {c.title}\nDescription: {c.description}\nSpeakers: {string.Join(", ", c.speakers)}\n")];
    }
}
