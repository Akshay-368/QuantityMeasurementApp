using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Kilogram : Weight
{
    public Kilogram(double value) : base(value, Unit.Kilogram)
    {
    }
}