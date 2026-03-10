using MediatR;
using QuantityMeasurement.Application.DTOs;

namespace QuantityMeasurement.Application.Features.Quantities;
public record CompareQuantitiesQuery(string Category, QuantityDto First, QuantityDto Second) : IRequest<bool>;