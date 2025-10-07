using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repositories;
using TaxCalculator.Domain.Services;

namespace TaxCalculator.Application.TaxCalculation.Services;
public class TaxCalculatorService : ITaxCalculatorService
{
    private readonly ITaxBandRepository _taxBandRepository;

    public TaxCalculatorService(ITaxBandRepository taxBandRepository)
    {
        _taxBandRepository = taxBandRepository;
    }

    public async Task<TaxCalculationResult> CalculateAsync(decimal grossSalary, CancellationToken cancellationToken)
    {
        if (grossSalary < 0)
            throw new ArgumentException("Gross salary cannot be negative.");

        // Simulate async fetch of tax bands (replace with real async repo call if available)
        var taxBands = await _taxBandRepository.GetTaxBandsAsync(cancellationToken);

        if (taxBands == null || !taxBands.Any())
        {
            return new TaxCalculationResult
            {
                GrossAnnualSalary = grossSalary,
                GrossMonthlySalary = grossSalary / 12m,
                NetAnnualSalary = grossSalary,
                NetMonthlySalary = grossSalary / 12m,
                AnnualTaxPaid = 0,
                MonthlyTaxPaid = 0
            };
        }

        var orderedBands = taxBands.OrderBy(b => b.LowerLimit).ToList();
        decimal totalTax = 0;

        for (int i = 0; i < orderedBands.Count; i++)
        {
            var band = orderedBands[i];
            TaxBandValidator.Validate(band);
            decimal upperLimit = (i < orderedBands.Count - 1) ? orderedBands[i + 1].LowerLimit : grossSalary;

            if (grossSalary > band.LowerLimit)
            {
                var taxableAmount = Math.Min(grossSalary, upperLimit) - band.LowerLimit;
                var taxForBand = taxableAmount * (band.TaxRate / 100);
                totalTax += taxForBand;
            }
        }

        var netAnnual = grossSalary - totalTax;

        return new TaxCalculationResult
        {
            GrossAnnualSalary = Math.Round(grossSalary, 2),
            GrossMonthlySalary = Math.Round(grossSalary / 12m, 2),
            NetAnnualSalary = Math.Round(netAnnual, 2),
            NetMonthlySalary = Math.Round(netAnnual / 12m, 2),
            AnnualTaxPaid = Math.Round(totalTax, 2),
            MonthlyTaxPaid = Math.Round(totalTax / 12m, 2)
        };
    }
}

