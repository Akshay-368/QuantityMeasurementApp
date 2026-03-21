using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Kelvin : Temperature
{
    public Kelvin(double value)
        : base(value, Unit.Kelvin)
    {
    }
}