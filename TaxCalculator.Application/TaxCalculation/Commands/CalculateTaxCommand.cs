using MediatR;
using TaxCalculator.Domain.Services;

namespace TaxCalculator.Application.TaxCalculation.Commands
{
    public class CalculateTaxCommand : IRequest<TaxResultDto>
    {
        public decimal GrossSalary { get; set; }
    }

    public class CalculateTaxCommandHandler : IRequestHandler<CalculateTaxCommand, TaxResultDto>
    {
        private readonly ITaxCalculatorService _taxCalculatorService;

        public CalculateTaxCommandHandler(ITaxCalculatorService taxCalculatorService)
        {
            _taxCalculatorService = taxCalculatorService;
        }

        public async Task<TaxResultDto> Handle(CalculateTaxCommand request, CancellationToken cancellationToken)
        {
            var calculationResult = await _taxCalculatorService.CalculateAsync(request.GrossSalary, cancellationToken);
            return TaxResultDto.MapFrom(calculationResult);
        }
    }
}