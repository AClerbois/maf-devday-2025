namespace DevDayContentMcp.Models;

public class SessionizeResponse
{
    public Session[] sessions { get; set; }
    public Speaker[] speakers { get; set; }
    public Question[] questions { get; set; }
    public Category[] categories { get; set; }
    public Room[] rooms { get; set; }
}

public class Session
{
    public string id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime startsAt { get; set; }
    public DateTime endsAt { get; set; }
    public bool isServiceSession { get; set; }
    public bool isPlenumSession { get; set; }
    public string[] speakers { get; set; }
    public int[] categoryItems { get; set; }
    public object[] questionAnswers { get; set; }
    public int roomId { get; set; }
    public object liveUrl { get; set; }
    public object recordingUrl { get; set; }
    public string status { get; set; }
    public bool isInformed { get; set; }
    public bool isConfirmed { get; set; }
}

public class Speaker
{
    public string id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string bio { get; set; }
    public string tagLine { get; set; }
    public string profilePicture { get; set; }
    public bool isTopSpeaker { get; set; }
    public Link[] links { get; set; }
    public int[] sessions { get; set; }
    public string fullName { get; set; }
    public object[] categoryItems { get; set; }
    public Questionanswer[] questionAnswers { get; set; }
}

public class Link
{
    public string title { get; set; }
    public string url { get; set; }
    public string linkType { get; set; }
}

public class Questionanswer
{
    public int questionId { get; set; }
    public string answerValue { get; set; }
}

public class Question
{
    public int id { get; set; }
    public string question { get; set; }
    public string questionType { get; set; }
    public int sort { get; set; }
}

public class Category
{
    public int id { get; set; }
    public string title { get; set; }
    public Item[] items { get; set; }
    public int sort { get; set; }
    public string type { get; set; }
}

public class Item
{
    public int id { get; set; }
    public string name { get; set; }
    public int sort { get; set; }
}

public class Room
{
    public int id { get; set; }
    public string name { get; set; }
    public int sort { get; set; }
}
