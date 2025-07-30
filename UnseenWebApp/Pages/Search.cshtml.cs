using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnseenWebApp.Data;
using UnseenWebApp.Models;
using UnseenWebApp.Services;

namespace UnseenWebApp.Pages;

public class SearchModel : PageModel
{
    private readonly ILogger<SearchModel> _logger;
    private readonly DataService _dataService;

    public SearchModel(ILogger<SearchModel> logger, DataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public const int PageSize = 10;

    public PagedResult<TopScoreUniqueStringEntity> SearchResults { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var (items, totalCount) = await _dataService.SearchWordsAsync(
                SearchTerm,
                CurrentPage,
                PageSize);

            SearchResults = new PagedResult<TopScoreUniqueStringEntity>
            {
                Items = items,
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                TotalCount = totalCount,
                SearchTerm = SearchTerm
            };

            _logger.LogInformation("Search performed: Term='{SearchTerm}', Page={CurrentPage}, Results={Count}",
                SearchTerm, CurrentPage, items.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching words");
            ErrorMessage = "An error occurred while searching. Please try again.";
            SearchResults = new PagedResult<TopScoreUniqueStringEntity>
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                SearchTerm = SearchTerm
            };
        }
    }

    public IActionResult OnPostSearch()
    {
        return RedirectToPage("/Search", new { SearchTerm, CurrentPage = 1 });
    }
}

