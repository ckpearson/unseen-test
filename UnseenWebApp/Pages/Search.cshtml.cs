using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnseenWebApp.Pages;

public class SearchModel : PageModel
{
    private readonly ILogger<SearchModel> _logger;

    public SearchModel(ILogger<SearchModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

