using MediatR;
using QuantityMeasurement.Application;
using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Application.Features.Quantities;

public class DivideQuantitiesHandler : IRequestHandler<DivideQuantitiesQuery, double>
{
    public Task<double> Handle(DivideQuantitiesQuery request, CancellationToken ct)
    {
        var unitDividend = QuantityFactory.GetUnitByName(request.Category, request.Dividend.UnitName);
        var dividend = QuantityFactory.CreateQuantity(request.Dividend.Value, unitDividend);

        var unitDivisor = QuantityFactory.GetUnitByName(request.Category, request.Divisor.UnitName);
        var divisorQty = QuantityFactory.CreateQuantity(request.Divisor.Value, unitDivisor);

        var ratio = dividend.Divide(divisorQty);

        return Task.FromResult(ratio);
    }
}