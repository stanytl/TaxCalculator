using Microsoft.EntityFrameworkCore;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Infrastructure.Configurations;

namespace TaxCalculator.Infrastructure;

public class TaxCalculatorDbContext : DbContext
{
    public DbSet<TaxBand> TaxBands { get; set; } = default!;

    public TaxCalculatorDbContext(DbContextOptions<TaxCalculatorDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaxBandConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public void SeedTaxBands()
    {
        if (!TaxBands.Any())
        {
            TaxBands.AddRange(
                new TaxBand { Id = 1, TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 },
                new TaxBand { Id = 2, TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 },
                new TaxBand { Id = 3, TaxCode = "C", LowerLimit = 20000, TaxRate = 40, TaxYear = 2025 }
            );
            SaveChanges();
        }
    }
}