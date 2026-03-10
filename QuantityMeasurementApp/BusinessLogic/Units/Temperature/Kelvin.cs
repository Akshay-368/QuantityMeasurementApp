using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Kelvin : Temperature
{
    public Kelvin(double value)
        : base(value, Unit.Kelvin)
    {
    }
}