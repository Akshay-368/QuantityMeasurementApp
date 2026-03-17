using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Kilogram : Weight
{
    public Kilogram(double value) : base(value, Unit.Kilogram)
    {
    }
}