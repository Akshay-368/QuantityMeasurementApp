using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

/// <summary>
/// Concrete Volume unit: Litre
/// </summary>
public class Litre : Volume
{
    public Litre(double value) : base(value, Unit.Litre)
    {
    }
}