using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TaxCalculator.Infrastructure.Repositories;

public class TaxBandRepository : ITaxBandRepository
{
    private readonly TaxCalculatorDbContext _dbContext;

    public TaxBandRepository(TaxCalculatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaxBand?> GetTaxBandAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.TaxBands
            .FirstOrDefaultAsync(tb => tb.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TaxBand>> GetTaxBandsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.TaxBands
            .OrderBy(tb => tb.LowerLimit)
            .ToListAsync(cancellationToken);
    }
}