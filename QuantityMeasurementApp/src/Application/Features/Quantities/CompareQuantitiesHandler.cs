using MediatR;
using QuantityMeasurement.Application;
using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Application.Features.Quantities;

public class CompareQuantitiesHandler : IRequestHandler<CompareQuantitiesQuery, bool>
{
    public Task<bool> Handle(CompareQuantitiesQuery request, CancellationToken cancellationToken)
    {
        // Copy from Menu's HandleComparison + ReadQuantity
        var unit1 = QuantityFactory.GetUnitByName(request.Category, request.First.UnitName);
        var q1 = QuantityFactory.CreateQuantity(request.First.Value, unit1);

        var unit2 = QuantityFactory.GetUnitByName(request.Category, request.Second.UnitName);
        var q2 = QuantityFactory.CreateQuantity(request.Second.Value, unit2);

        // Domain call—unchanged!
        return Task.FromResult(q1.Equals(q2));
    }
}