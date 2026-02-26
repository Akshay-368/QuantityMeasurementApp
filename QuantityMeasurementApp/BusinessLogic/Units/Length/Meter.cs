namespace QuantityMeasurement.Units;
using QuantityMeasurement.Core;
public class Meter : Length
{
    public Meter(double value) : base(value, Unit.Meter)
    {
    }
}