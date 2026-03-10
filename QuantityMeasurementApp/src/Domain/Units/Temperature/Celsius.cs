using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Celsius : Temperature
{
    public Celsius(double value)
        : base(value, Unit.Celsius)
    {
    }
}