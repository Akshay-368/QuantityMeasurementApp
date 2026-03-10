using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Celsius : Temperature
{
    public Celsius(double value)
        : base(value, Unit.Celsius)
    {
    }
}