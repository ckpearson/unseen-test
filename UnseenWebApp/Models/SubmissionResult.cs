using System;
using System.Collections.Immutable;

namespace UnseenWebApp.Models;

/// <summary>
/// Discrimination union (ish) type for submission results
/// </summary>
public abstract record SubmissionResult
{
    public required string Input { get; init; }
}

public sealed record SuccessSubmissionResult : SubmissionResult
{
    /// <summary>
    /// The extracted value that was successfully submitted
    /// </summary>
    public required string Value { get; init; }
}

public sealed record ErrorSubmissionResult : SubmissionResult
{
    /// <summary>
    /// The error message describing why the submission failed
    /// </summary>
    public required string Message { get; init; }
}
