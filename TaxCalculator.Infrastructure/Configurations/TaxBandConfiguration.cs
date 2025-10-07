using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Infrastructure.Configurations;

public class TaxBandConfiguration : IEntityTypeConfiguration<TaxBand>
{
    public void Configure(EntityTypeBuilder<TaxBand> builder)
    {
        builder.HasKey(tb => tb.Id);

        builder.Property(tb => tb.TaxCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(tb => tb.LowerLimit)
            .IsRequired();

        builder.Property(tb => tb.TaxRate)
            .IsRequired();

        builder.Property(tb => tb.TaxYear)
            .IsRequired();

        builder.HasIndex(b => b.LowerLimit).IsUnique();
        builder.HasIndex(b => b.TaxCode).IsUnique(); // Add this line
    }
}