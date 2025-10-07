using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Domain.Repositories;

public interface ITaxBandRepository
{
    Task<TaxBand?> GetTaxBandAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<TaxBand>> GetTaxBandsAsync(CancellationToken cancellationToken);
}
