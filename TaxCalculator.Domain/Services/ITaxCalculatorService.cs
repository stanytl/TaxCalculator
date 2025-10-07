using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Domain.Services;
public interface ITaxCalculatorService
{
    Task<TaxCalculationResult> CalculateAsync(decimal grossSalary, CancellationToken cancellationToken);
}