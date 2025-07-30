using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using UnseenWebApp.Models;
using UnseenWebApp.Services;

namespace UnseenWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly DataService _dataService;

    public IndexModel(ILogger<IndexModel> logger, DataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Please enter a string")]
    public string InputString { get; set; } = string.Empty;

    [TempData]
    public string? Message { get; set; }

    [TempData]
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please correct the errors and try again.";
            IsSuccess = false;
            return Page();
        }

        try
        {
            _logger.LogInformation("Received string: {InputString}", InputString);

            switch (await _dataService.SubmitTopScoreWordAsync(InputString))
            {
                case SuccessSubmissionResult success:
                    Message = $"Value '{success.Value}' submitted successfully!";
                    IsSuccess = true;
                    break;
                case ErrorSubmissionResult error:
                    Message = error.Message;
                    IsSuccess = false;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected submission result type.");
            }

            InputString = string.Empty;
            ModelState.Clear();

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing string submission");
            Message = "An error occurred while processing your submission. Please try again.";
            IsSuccess = false;
            return Page();
        }
    }
}
