using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Application.TaxCalculation.Services;

public static class TaxBandValidator
{
    public static void Validate(TaxBand band)
    {
        if (band == null)
            throw new ArgumentNullException(nameof(band));

        if (string.IsNullOrWhiteSpace(band.TaxCode) || band.TaxCode.Length < 1 || band.TaxCode.Length > 3)
            throw new ArgumentException("Tax code must be between 1 and 3 characters.");

        if (band.LowerLimit < 0)
            throw new ArgumentException("Lower limit must be zero or greater.");

        if (band.TaxRate < 0 || band.TaxRate > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100 percent.");

        if (band.TaxYear < 2000 || band.TaxYear > 2100)
            throw new ArgumentException("Tax year must be a valid year between 2000 and 2100.");
    }
}