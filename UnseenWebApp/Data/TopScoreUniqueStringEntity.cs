using System;

namespace UnseenWebApp.Data;

public class TopScoreUniqueStringEntity
{
    public int Id { get; set; }
    public required string Word { get; set; }
    public required DateTimeOffset SubmittedAtUtc { get; set; }
}
