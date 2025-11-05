using System.ComponentModel;

namespace Maf;

public record SpeakerInfo(
    [property: Description("The first name of the speaker.")] string FirstName,
    [property: Description("The last name of the speaker.")] string LastName,
    [property: Description("The talk title of the speaker.")] string TalkTitle
);