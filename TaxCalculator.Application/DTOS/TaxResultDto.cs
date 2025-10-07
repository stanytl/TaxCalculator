using TaxCalculator.Domain.Entities;

public class TaxResultDto
{
    public decimal GrossAnnualSalary { get; set; }
    public decimal GrossMonthlySalary { get; set; }
    public decimal NetAnnualSalary { get; set; }
    public decimal NetMonthlySalary { get; set; }
    public decimal AnnualTaxPaid { get; set; }
    public decimal MonthlyTaxPaid { get; set; }

    public static TaxResultDto MapFrom(TaxCalculationResult result)
    {
        return new TaxResultDto
        {
            GrossAnnualSalary = result.GrossAnnualSalary,
            GrossMonthlySalary = result.GrossMonthlySalary,
            NetAnnualSalary = result.NetAnnualSalary,
            NetMonthlySalary = result.NetMonthlySalary,
            AnnualTaxPaid = result.AnnualTaxPaid,
            MonthlyTaxPaid = result.MonthlyTaxPaid
        };
    }
}