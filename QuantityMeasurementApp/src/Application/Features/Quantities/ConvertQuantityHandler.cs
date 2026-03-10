using MediatR;
using QuantityMeasurement.Application.DTOs;
using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Application.Features.Quantities;

public class ConvertQuantityHandler : IRequestHandler<ConvertQuantityQuery, ResultDto>
{
    public Task<ResultDto> Handle(ConvertQuantityQuery request, CancellationToken ct)
    {
        var sourceUnit = QuantityFactory.GetUnitByName(request.Category, request.Source.UnitName);
        var sourceQty = QuantityFactory.CreateQuantity(request.Source.Value, sourceUnit);

        var targetUnit = QuantityFactory.GetUnitByName(request.Category, request.TargetUnitName);

        var converted = sourceQty.ConvertTo(targetUnit);

        return Task.FromResult(new ResultDto(
            converted.value,
            converted.unit.Name,
            converted.ToString()
        ));
    }
}