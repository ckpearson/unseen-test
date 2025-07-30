using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using UnseenWebApp.Data;
using UnseenWebApp.Models;

namespace UnseenWebApp.Services;

public class DataService(UnseenWebAppDbContext dbContext, ILogger<DataService> logger)
{
    private readonly UnseenWebAppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<DataService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static ImmutableList<Range> FindCandidateWords(ReadOnlySpan<char> inputSpan)
    {
        /*
        Track the current cohort of words that are the longest valid candidates encountered.
        */
        var cohort = new HashSet<Range>();
        var currentLongest = 0;

        foreach (var wordRange in inputSpan.Split(' '))
        {
            // Track chars seen in the current word to avoid repeats
            var seenChars = new HashSet<char>();
            var hasUpper = false;
            var hasLower = false;
            var hasDigit = false;
            var hasNoRepeats = true;

            var (offset, length) = wordRange.GetOffsetAndLength(inputSpan.Length);
            if (length < 8)
            {
                continue; // Skip words shorter than 8 characters
            }

            if (length < currentLongest)
            {
                // This word is shorter than the current longest candidate cohort, skip it
                continue;
            }

            /*
            Enumerate the characters in the word to the end of the word

            - No early exit even if we've met criteria, as we can't guarantee the character uniqueness requirement until we've checked everything.
            */
            foreach (var ch in inputSpan.Slice(offset, length))
            {
                if (seenChars.Contains(ch))
                {
                    hasNoRepeats = false;
                    break;
                }

                seenChars.Add(ch);

                if (char.IsUpper(ch))
                {
                    hasUpper = true;
                }
                else if (char.IsLower(ch))
                {
                    hasLower = true;
                }
                else if (char.IsDigit(ch))
                {
                    hasDigit = true;
                }
            }

            if (hasUpper && hasLower && hasDigit && hasNoRepeats)
            {
                // This word meets all criteria, and is longer than the current cohort, so reset
                if (length > currentLongest)
                {
                    cohort = [wordRange];
                    currentLongest = length;
                }
                else if (length == currentLongest)
                {
                    // Word is also valid and same length as the current cohort, so add it
                    cohort.Add(wordRange);
                }
                else
                {
                    // Word is valid, but shorter than the current cohort, skip just in case
                    continue;
                }
            }
        }

        // No valid candidates found
        if (cohort.Count == 0)
        {
            return [];
        }

        return [.. cohort];
    }

    public async Task<SubmissionResult> SubmitTopScoreWordAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ErrorSubmissionResult
            {
                Input = input,
                Message = "Input cannot be empty."
            };
        }


        // Our string could be arbitrarily long, so let's treat it as a span so we can process it with as few allocations as possible.
        ReadOnlySpan<char> inputSpan = input.AsSpan();

        var proposedCandidates = FindCandidateWords(inputSpan);

        if (proposedCandidates.Count == 0)
        {
            return new ErrorSubmissionResult
            {
                Input = input,
                Message = "No valid candidates found in the input string."
            };
        }

        /*
        Assumption:

        If we have multiple candidates, we will pick one at random to submit.

        Alternative:

        We could consult the database to see if any already exist and exclude those.
        */

        var chosenCandidate =
            proposedCandidates.Count == 1
                ? proposedCandidates[0]
                : proposedCandidates.OrderBy(_ => Random.Shared.Next()).First();

        // Convert the chosen candidate range back to a string
        var (offset, length) = chosenCandidate.GetOffsetAndLength(inputSpan.Length);
        var chosenWord = inputSpan.Slice(offset, length).ToString();

        if (await _dbContext.TopScoreUniqueStrings.AnyAsync(e => e.Word == chosenWord))
        {
            return new ErrorSubmissionResult
            {
                Input = input,
                Message = $"The word '{chosenWord}' has already been submitted."
            };
        }

        var entity = new TopScoreUniqueStringEntity
        {
            Word = chosenWord,
            SubmittedAtUtc = DateTime.UtcNow
        };

        _dbContext.TopScoreUniqueStrings.Add(entity);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving changes to the database.");
            return new ErrorSubmissionResult
            {
                Input = input,
                Message = "An error occurred while saving your submission."
            };
        }

        return new SuccessSubmissionResult
        {
            Input = input,
            Value = chosenWord
        };
    }

    public async Task<(IEnumerable<TopScoreUniqueStringEntity> Items, int TotalCount)> SearchWordsAsync(
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _dbContext.TopScoreUniqueStrings.AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
            query = query.Where(w => w.Word.ToLower().Contains(lowerSearchTerm));
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var items = await query
            .OrderByDescending(w => w.SubmittedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
