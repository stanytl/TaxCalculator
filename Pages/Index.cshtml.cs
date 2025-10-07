using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxCalculator.Application.TaxCalculation.Services;

public class IndexModel : PageModel
{
    private readonly ITaxCalculatorService _taxCalculatorService;

    [BindProperty]
    public decimal GrossSalary { get; set; }

    public TaxCalculationResult? Result { get; set; }

    public IndexModel(ITaxCalculatorService taxCalculatorService)
    {
        _taxCalculatorService = taxCalculatorService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Result = await _taxCalculatorService.CalculateAsync(GrossSalary, HttpContext.RequestAborted);
        return Page();
    }
}