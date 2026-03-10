using MediatR;
using QuantityMeasurement.Application.DTOs;
using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Application.Features.Quantities;

public class AddQuantitiesHandler : IRequestHandler<AddQuantitiesCommand, ResultDto>
{
    public Task<ResultDto> Handle(AddQuantitiesCommand request, CancellationToken ct)
    {
        var unit1 = QuantityFactory.GetUnitByName(request.Category, request.First.UnitName);
        var q1 = QuantityFactory.CreateQuantity(request.First.Value, unit1);

        var unit2 = QuantityFactory.GetUnitByName(request.Category, request.Second.UnitName);
        var q2 = QuantityFactory.CreateQuantity(request.Second.Value, unit2);

        Quantity result;

        if (string.IsNullOrEmpty(request.TargetUnitName))
        {
            // mode 1: result in first unit
            result = q1.Add(q2);
        }
        else
        {
            // mode 2: explicit target unit
            var targetUnit = QuantityFactory.GetUnitByName(request.Category, request.TargetUnitName);
            result = q1.Add(q2, targetUnit);
        }

        return Task.FromResult(new ResultDto(
            result.value,
            result.unit.Name,
            result.ToString()
        ));
    }
}