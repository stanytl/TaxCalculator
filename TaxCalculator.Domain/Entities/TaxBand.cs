namespace TaxCalculator.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class TaxBand
{
    public int Id { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 1, ErrorMessage = "Tax code must be between 1 and 3 characters.")]
    public string TaxCode { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Lower limit must be zero or greater.")]
    public decimal LowerLimit { get; set; }

    [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100 percent.")]
    public decimal TaxRate { get; set; }

    [Range(2000, 2100, ErrorMessage = "Tax year must be a valid year between 2000 and 2100.")]
    public int TaxYear { get; set; }
}
