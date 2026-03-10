using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;

public record DivideQuantitiesQuery(
    string Category,
    QuantityDto Dividend,
    QuantityDto Divisor
) : IRequest<double>;