namespace TaxCalculator.Domain.Entities;

public class TaxBand
{
    public int Id { get; set; }

    public string TaxCode { get; set; } = string.Empty;

    public decimal LowerLimit { get; set; }
        
    public decimal TaxRate { get; set; }
        
    public int TaxYear { get; set; }
}
