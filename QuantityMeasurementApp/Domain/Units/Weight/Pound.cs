using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Pound : Weight
{
    public Pound(double value) : base(value, Unit.Pound)
    {
    }
}