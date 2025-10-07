using MediatR;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repositories;

public record GetAllTaxBandsQuery() : IRequest<IEnumerable<TaxBand>>;

public class GetAllTaxBandsQueryHandler : IRequestHandler<GetAllTaxBandsQuery, IEnumerable<TaxBand>>
{
    private readonly ITaxBandRepository _repository;

    public GetAllTaxBandsQueryHandler(ITaxBandRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaxBand>> Handle(GetAllTaxBandsQuery request, CancellationToken cancellationToken)
    {
        var taxBands =  await _repository.GetTaxBandsAsync(cancellationToken);
        return taxBands;
    }
}