using MediatR;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repositories;

public record GetTaxBandByIdQuery(int Id) : IRequest<TaxBand?>;

public class GetTaxBandByIdQueryHandler : IRequestHandler<GetTaxBandByIdQuery, TaxBand?>
{
    private readonly ITaxBandRepository _repository;

    public GetTaxBandByIdQueryHandler(ITaxBandRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaxBand?> Handle(GetTaxBandByIdQuery request, CancellationToken cancellationToken)
    {
        var taxBand = await _repository.GetTaxBandAsync(request.Id, cancellationToken);
        return taxBand;
    }
}