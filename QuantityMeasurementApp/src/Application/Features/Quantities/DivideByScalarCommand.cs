using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;

public record DivideByScalarCommand(
    string Category,
    QuantityDto Quantity,
    double Divisor
) : IRequest<ResultDto>;