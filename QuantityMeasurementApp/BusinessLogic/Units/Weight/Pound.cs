using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Pound : Weight
{
    public Pound(double value) : base(value, Unit.Pound)
    {
    }
}