namespace QuantityMeasurement.Domain.Units;
using QuantityMeasurement.Domain.Core;
public class Yard : Length
{
    public Yard(double value) : base(value, Unit.Yard)
    {
    }
}