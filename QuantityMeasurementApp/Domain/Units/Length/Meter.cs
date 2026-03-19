namespace QuantityMeasurement.Domain.Units;
using QuantityMeasurement.Domain.Core;
public class Meter : Length
{
    public Meter(double value) : base(value, Unit.Meter)
    {
    }
}