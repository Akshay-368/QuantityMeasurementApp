// ResultDto.cs
namespace QuantityMeasurement.Application.DTOs;

public record ResultDto(
    double Value,
    string UnitName,
    string Display             // e.g. "12.0000 Feet"
);