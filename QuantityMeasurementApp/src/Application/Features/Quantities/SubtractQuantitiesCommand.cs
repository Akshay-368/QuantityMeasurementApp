using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;

public record SubtractQuantitiesCommand(
    string Category,
    QuantityDto First,
    QuantityDto Second,
    string? TargetUnitName = null
) : IRequest<ResultDto>;