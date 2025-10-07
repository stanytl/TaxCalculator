using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Infrastructure;
using System;
using System.Threading.Tasks;


namespace TaxCalculator.Tests.Repository
{
    [TestClass]
    public class TaxBandRepositoryTests
    {
        private DbContextOptions<TaxCalculatorDbContext> CreateSqliteOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<TaxCalculatorDbContext>()
                .UseSqlite(connection)
                .Options;
        }

        [TestMethod]
        public async Task AddingDuplicateTaxCode_ThrowsDbUpdateException()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();
                var options = CreateSqliteOptions(connection);

                using (var context = new TaxCalculatorDbContext(options))
                {
                    context.Database.EnsureCreated();

                    var band1 = new TaxBand { TaxCode = "A1", LowerLimit = 1000, TaxRate = 20, TaxYear = 2025 };
                    var band2 = new TaxBand { TaxCode = "A1", LowerLimit = 2000, TaxRate = 30, TaxYear = 2025 };

                    context.TaxBands.Add(band1);
                    await context.SaveChangesAsync();

                    context.TaxBands.Add(band2);
                    await Assert.ThrowsExactlyAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
                }
            }
        }

        [TestMethod]
        public async Task AddingDuplicateLowerLimit_ThrowsDbUpdateException()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();
                var options = CreateSqliteOptions(connection);

                using (var context = new TaxCalculatorDbContext(options))
                {
                    context.Database.EnsureCreated();

                    var band1 = new TaxBand { TaxCode = "A1", LowerLimit = 1000, TaxRate = 20, TaxYear = 2025 };
                    var band2 = new TaxBand { TaxCode = "B1", LowerLimit = 1000, TaxRate = 30, TaxYear = 2025 };

                    context.TaxBands.Add(band1);
                    await context.SaveChangesAsync();

                    context.TaxBands.Add(band2);
                    await Assert.ThrowsExactlyAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
                }
            }
        }
    }
}