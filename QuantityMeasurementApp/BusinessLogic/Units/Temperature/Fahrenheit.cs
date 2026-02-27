using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Fahrenheit : Temperature
{
    public Fahrenheit(double value)
        : base(value, Unit.Fahrenheit)
    {
    }
}