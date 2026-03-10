using MediatR;
using QuantityMeasurement.Application.DTOs;
using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Application.Features.Quantities;

public class DivideByScalarHandler : IRequestHandler<DivideByScalarCommand, ResultDto>
{
    public Task<ResultDto> Handle(DivideByScalarCommand request, CancellationToken ct)
    {
        var unit = QuantityFactory.GetUnitByName(request.Category, request.Quantity.UnitName);
        var qty = QuantityFactory.CreateQuantity(request.Quantity.Value, unit);

        var result = qty.Divide(request.Divisor);

        return Task.FromResult(new ResultDto(
            result.value,
            result.unit.Name,
            result.ToString()
        ));
    }
}