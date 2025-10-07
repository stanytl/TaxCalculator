using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Application.TaxCalculation.Services;
using System;

namespace TaxCalculator.Tests.Service
{
    [TestClass]
    public class TaxBandValidatorTests
    {
        [TestMethod]
        public void Validate_NullTaxBand_ThrowsArgumentNullExceptionWithMessage()
        {
            var ex = Assert.ThrowsExactly<ArgumentNullException>(() =>
                TaxBandValidator.Validate(null!));
            StringAssert.Contains(ex.Message, "Value cannot be null. (Parameter 'band')");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("ABCD")]
        public void Validate_InvalidTaxCode_ThrowsArgumentExceptionWithMessage(string taxCode)
        {
            var band = new TaxBand
            {
                TaxCode = taxCode,
                LowerLimit = 0,
                TaxRate = 20,
                TaxYear = 2025
            };

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                TaxBandValidator.Validate(band));
            Assert.AreEqual("Tax code must be between 1 and 3 characters.", ex.Message);
        }

        [TestMethod]
        public void Validate_NegativeLowerLimit_ThrowsArgumentExceptionWithMessage()
        {
            var band = new TaxBand
            {
                TaxCode = "A1",
                LowerLimit = -1,
                TaxRate = 20,
                TaxYear = 2025
            };

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                TaxBandValidator.Validate(band));
            Assert.AreEqual("Lower limit must be zero or greater.", ex.Message);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void Validate_InvalidTaxRate_ThrowsArgumentExceptionWithMessage(int taxRate)
        {
            var band = new TaxBand
            {
                TaxCode = "A1",
                LowerLimit = 0,
                TaxRate = taxRate,
                TaxYear = 2025
            };

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                TaxBandValidator.Validate(band));
            Assert.AreEqual("Tax rate must be between 0 and 100 percent.", ex.Message);
        }

        [TestMethod]
        [DataRow(1999)]
        [DataRow(2101)]
        public void Validate_InvalidTaxYear_ThrowsArgumentExceptionWithMessage(int taxYear)
        {
            var band = new TaxBand
            {
                TaxCode = "A1",
                LowerLimit = 0,
                TaxRate = 20,
                TaxYear = taxYear
            };

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                TaxBandValidator.Validate(band));
            Assert.AreEqual("Tax year must be a valid year between 2000 and 2100.", ex.Message);
        }

        [TestMethod]
        public void Validate_ValidTaxBand_DoesNotThrow()
        {
            var band = new TaxBand
            {
                TaxCode = "A1",
                LowerLimit = 0,
                TaxRate = 20,
                TaxYear = 2025
            };

            TaxBandValidator.Validate(band); // Should not throw
        }
    }
}