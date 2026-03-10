using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;

public record ConvertQuantityQuery(
    string Category,
    QuantityDto Source,
    string TargetUnitName
) : IRequest<ResultDto>;