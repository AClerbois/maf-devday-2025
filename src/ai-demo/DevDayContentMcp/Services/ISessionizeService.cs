using DevDayContentMcp.Models;

namespace DevDayContentMcp.Services;

public interface ISessionizeService
{
    IList<Session> GetSessions();
    IList<Speaker> GetSpeakers();
}