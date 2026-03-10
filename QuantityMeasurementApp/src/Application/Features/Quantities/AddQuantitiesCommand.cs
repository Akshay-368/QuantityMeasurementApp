using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;

public record AddQuantitiesCommand(
    string Category,
    QuantityDto First,
    QuantityDto Second,
    string? TargetUnitName = null   // null = use First's unit
) : IRequest<ResultDto>;