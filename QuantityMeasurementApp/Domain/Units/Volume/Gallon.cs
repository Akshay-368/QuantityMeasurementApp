using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

/// <summary>
/// Concrete Volume unit: Gallon
/// </summary>
public class Gallon : Volume
{
    public Gallon(double value) : base(value, Unit.Gallon)
    {
    }
}