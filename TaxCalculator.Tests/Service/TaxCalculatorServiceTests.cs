using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Application.TaxCalculation.Services;
using Moq;
using TaxCalculator.Domain.Repositories;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaxCalculator.Test.Service
{
    [TestClass]
    public class CalculateAsync
    {
        private Mock<ITaxBandRepository> _taxBandRepositoryMock = null!;
        private TaxCalculatorService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _taxBandRepositoryMock = new Mock<ITaxBandRepository>();
            _service = new TaxCalculatorService(_taxBandRepositoryMock.Object);
        }

        [TestMethod]
        public async Task CalculateAsync_WithSalary10000_ReturnsCorrectTax()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 },
                new TaxBand { TaxCode = "C", LowerLimit = 20000, TaxRate = 40, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 10000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(9000m, result.NetAnnualSalary);  // 10000 - 1000 tax
            Assert.AreEqual(750m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(1000m, result.AnnualTaxPaid);
            Assert.AreEqual(83.33m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryAtBandEdge_ReturnsCorrectTax()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 5000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(5000m, result.GrossAnnualSalary);
            Assert.AreEqual(416.67m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(5000m, result.NetAnnualSalary); // No tax at edge
            Assert.AreEqual(416.67m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_EmptyTaxBands_ReturnsGrossAsNet()
        {
            // Arrange
            var taxBands = new List<TaxBand>();
            decimal grossSalary = 10000m;
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(taxBands);

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(10000m, result.NetAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_NullTaxBands_HandlesGracefully()
        {
            // Arrange
            decimal grossSalary = 10000m;
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<TaxBand>?)null!);

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(10000m, result.NetAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_SingleBandCoveringAllIncome()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 10000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(9000m, result.NetAnnualSalary);
            Assert.AreEqual(750m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(1000m, result.AnnualTaxPaid);
            Assert.AreEqual(83.33m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_MultipleBands_PartialCalculations()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 10, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 10000m;
            // Band A: (5,000 - 0) * 10% = 500
            // Band B: (10,000 - 5,000) * 20% = 1,000
            // Total tax: 1,500
            // Net: 8,500

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(8500m, result.NetAnnualSalary);
            Assert.AreEqual(708.33m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(1500m, result.AnnualTaxPaid);
            Assert.AreEqual(125m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_MonthlyCalculations_RoundingPrecision()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 33.3333m, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 10000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(6666.67m, Math.Round(result.NetAnnualSalary, 2));
            Assert.AreEqual(555.56m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(3333.33m, Math.Round(result.AnnualTaxPaid, 2));
            Assert.AreEqual(277.78m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryAboveUpperBand_ReturnsCorrectTax()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 },
                new TaxBand { TaxCode = "C", LowerLimit = 20000, TaxRate = 40, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 25000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Calculation:
            // Band A: (5,000 - 0) * 0% = 0
            // Band B: (20,000 - 5,000) * 20% = 15,000 * 20% = 3,000
            // Band C: (25,000 - 20,000) * 40% = 5,000 * 40% = 2,000
            // Total tax: 3,000 + 2,000 = 5,000
            // Net: 25,000 - 5,000 = 20,000

            // Assert
            Assert.AreEqual(25000m, result.GrossAnnualSalary);
            Assert.AreEqual(2083.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(20000m, result.NetAnnualSalary); // Correct net
            Assert.AreEqual(1666.67m, Math.Round(result.NetMonthlySalary, 2)); // Correct net monthly
            Assert.AreEqual(5000m, result.AnnualTaxPaid); // Correct tax
            Assert.AreEqual(416.67m, Math.Round(result.MonthlyTaxPaid, 2)); // Correct monthly tax
        }

        [TestMethod]
        public async Task CalculateAsync_TaxBandsInIncorrectOrder_ReturnsCorrectTax()
        {
            // Arrange: Bands are out of order (highest lower limit first)
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "C", LowerLimit = 20000, TaxRate = 40, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 },
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 10000m;

            // Act
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Assert: Should match the correct calculation as if bands were ordered
            Assert.AreEqual(10000m, result.GrossAnnualSalary);
            Assert.AreEqual(833.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(9000m, result.NetAnnualSalary);  // 10000 - 1000 tax
            Assert.AreEqual(750m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(1000m, result.AnnualTaxPaid);
            Assert.AreEqual(83.33m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_NegativeSalary_ThrowsArgumentException()
        {
            // Arrange
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(taxBands);

            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
            {
                await _service.CalculateAsync(-5000m, CancellationToken.None);
            });
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryIsZero_ReturnsZeroTax()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 0m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            Assert.AreEqual(0m, result.GrossAnnualSalary);
            Assert.AreEqual(0m, result.GrossMonthlySalary);
            Assert.AreEqual(0m, result.NetAnnualSalary);
            Assert.AreEqual(0m, result.NetMonthlySalary);
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryBelowFirstBand_ReturnsZeroTax()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 1000, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 500m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            Assert.AreEqual(500m, result.GrossAnnualSalary);
            Assert.AreEqual(41.67m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(500m, result.NetAnnualSalary);
            Assert.AreEqual(41.67m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryAtBandLowerLimit_ReturnsZeroTaxForBand()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 1000, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 1000m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            Assert.AreEqual(1000m, result.GrossAnnualSalary);
            Assert.AreEqual(83.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(1000m, result.NetAnnualSalary);
            Assert.AreEqual(83.33m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0m, result.AnnualTaxPaid);
            Assert.AreEqual(0m, result.MonthlyTaxPaid);
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryJustAboveBandLowerLimit_CalculatesTaxOnExcess()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 1000, TaxRate = 10, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 1001m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            Assert.AreEqual(1001m, result.GrossAnnualSalary);
            Assert.AreEqual(83.42m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(1000.90m, Math.Round(result.NetAnnualSalary, 2));
            Assert.AreEqual(83.41m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(0.10m, Math.Round(result.AnnualTaxPaid, 2));
            Assert.AreEqual(0.01m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryIsVeryLarge_CalculatesWithoutOverflow()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 50, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 1000000000m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            Assert.AreEqual(1000000000m, result.GrossAnnualSalary);
            Assert.AreEqual(83333333.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(500000000m, result.NetAnnualSalary);
            Assert.AreEqual(41666666.67m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(500000000m, result.AnnualTaxPaid);
            Assert.AreEqual(41666666.67m, Math.Round(result.MonthlyTaxPaid, 2));
        }

        [TestMethod]
        public async Task CalculateAsync_SalaryWithMultipleBands_CorrectProgressiveTax()
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand { TaxCode = "A", LowerLimit = 0, TaxRate = 0, TaxYear = 2025 },
                new TaxBand { TaxCode = "B", LowerLimit = 5000, TaxRate = 20, TaxYear = 2025 },
                new TaxBand { TaxCode = "C", LowerLimit = 20000, TaxRate = 40, TaxYear = 2025 }
            };
            _taxBandRepositoryMock.Setup(r => r.GetTaxBandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(taxBands);

            decimal grossSalary = 40000m;
            var result = await _service.CalculateAsync(grossSalary, CancellationToken.None);

            // Band A: (5,000 - 0) * 0% = 0
            // Band B: (20,000 - 5,000) * 20% = 3,000
            // Band C: (40,000 - 20,000) * 40% = 8,000
            // Total tax: 11,000
            // Net: 29,000

            Assert.AreEqual(40000m, result.GrossAnnualSalary);
            Assert.AreEqual(3333.33m, Math.Round(result.GrossMonthlySalary, 2));
            Assert.AreEqual(29000m, result.NetAnnualSalary);
            Assert.AreEqual(2416.67m, Math.Round(result.NetMonthlySalary, 2));
            Assert.AreEqual(11000m, result.AnnualTaxPaid);
            Assert.AreEqual(916.67m, Math.Round(result.MonthlyTaxPaid, 2));
        }
    }
}