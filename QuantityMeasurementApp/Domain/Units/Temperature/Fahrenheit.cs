using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Fahrenheit : Temperature
{
    public Fahrenheit(double value)
        : base(value, Unit.Fahrenheit)
    {
    }
}